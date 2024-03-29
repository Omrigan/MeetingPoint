var mapContainer;
var  routeInstructionsContainer;
function draw_single(map, platform, place) {
	console.log(place)
		var pngIcon = new H.map.Icon("/crown.png",  {size: {w: 32, h: 32}});
	const coords = 		{
	  lat:place.place.coordinate.latitude,
	  lng:place.place.coordinate.longitude
	}

  var summaryDiv = document.createElement('div'),
   content = '<h1> О Месте </h1>';
   content += 'Название: ' + place.place.title + '<br>';
   content += 'Ссылка: <a href="' + place.place.href + '">' + place.place.href + '</a><br>';

  summaryDiv.style.fontSize = 'small';
  summaryDiv.style.marginLeft ='5%';
  summaryDiv.style.marginRight ='5%';
  summaryDiv.innerHTML = content;
  routeInstructionsContainer.appendChild(summaryDiv);

	var marker = new H.map.Marker(coords, {icon: pngIcon});
	map.setCenter(coords);
	console.log(marker);
	map.addObject(marker);
	place.memberRoutes.map(member => draw_member(map, platform, place, member))
}

function getGroupId() {
 const arr = window.location.href.split("/");
 return arr[arr.length-1];
}

function convertLocation(loc) {
	return loc.latitude + "," + loc.longitude
}

function draw_member(map, platform, place, member) {
	console.log("Member", member);
	var marker = new H.map.Marker({
	  lat:member.memberLocation.latitude,
	  lng:member.memberLocation.longitude
	});
	map.addObject(marker);
	draw_route(map, platform, convertLocation(member.memberLocation), 
		convertLocation(place.place.coordinate), member);

}



function draw_route(map, platform, from, to, member) {
	var router = platform.getRoutingService(),
	  routeRequestParams = {
	    mode: 'fastest;publicTransport',
	    representation: 'display',
	    routeattributes: 'waypoints,summary,shape,legs',
	    maneuverattributes: 'direction,action',
	    waypoint0: from, // Brandenburg Gate
	    waypoint1: to,  // Friedrichstraße Railway Station
	    language: 'ru-ru'
	  };
	function onSuccess(result) {
	 /*
	  * The styling of the route response on the map is entirely under the developer's control.
	  * A representitive styling can be found the full JS + HTML code of this example
	  * in the functions below:
	  */


	  console.log(member);
  var summaryDiv = document.createElement('div'),
   content = '<h1>' +  member.memberId + '</h1>';
		if(!result.response) {
		content+= 'Доехать не получится :(';
		}

  summaryDiv.style.fontSize = 'small';
  summaryDiv.style.marginLeft ='5%';
  summaryDiv.style.marginRight ='5%';
  summaryDiv.innerHTML = content;
  routeInstructionsContainer.appendChild(summaryDiv);
		if(!result.response) {
			return
		}

	  var route = result.response.route[0];

	  addRouteShapeToMap(map, route);
	  addWaypointsToPanel(route.waypoint);
	  addManueversToPanel(route);
	  addSummaryToPanel(route.summary);
	  // ... etc.
	}

	router.calculateRoute(
	  routeRequestParams,
	  onSuccess,
	  onError
	);

}

function draw_all(map, platform, data) {
	mapContainer = document.getElementById('map');
  routeInstructionsContainer = document.getElementById('panel');
	draw_single(map, platform, data[0])


}

function main(map, platform) {
	const data = fetch("http://meetingpointserver.azurewebsites.net/api/MeetingPoint/GetResult/"+getGroupId()).then(data => data.json()).then(data=> draw_all(map, platform, data));


}

/**
 * This function will be called once the Routing REST API provides a response
 * @param  {Object} result          A JSONP object representing the calculated route
 *
 * see: http://developer.here.com/rest-apis/documentation/routing/topics/resource-type-calculate-route.html
 */

/**
 * This function will be called if a communication error occurs during the JSON-P request
 * @param  {Object} error  The error message received.
 */
function onError(error) {
  alert('Can\'t reach the remote server');
}



// Hold a reference to any infobubble opened
var bubble;

/**
 * Opens/Closes a infobubble
 * @param  {H.geo.Point} position     The location on the map.
 * @param  {String} text              The contents of the infobubble.
 */
function openBubble(position, text){
 if(!bubble){
    bubble =  new H.ui.InfoBubble(
      position,
      // The FO property holds the province name.
      {content: text});
    ui.addBubble(bubble);
  } else {
    bubble.setPosition(position);
    bubble.setContent(text);
    bubble.open();
  }
}


/**
 * Creates a H.map.Polyline from the shape of the route and adds it to the map.
 * @param {Object} route A route as received from the H.service.RoutingService
 */
function addRouteShapeToMap(map, route){
  var lineString = new H.geo.LineString(),
    routeShape = route.shape,
    polyline;

  routeShape.forEach(function(point) {
    var parts = point.split(',');
    lineString.pushLatLngAlt(parts[0], parts[1]);
  });

  polyline = new H.map.Polyline(lineString, {
    style: {
      lineWidth: 4,
      strokeColor: 'rgba(0, 128, 255, 0.7)'
    }
  });
  // Add the polyline to the map
  map.addObject(polyline);
  // And zoom to its bounding rectangle
  //map.getViewModel().setLookAtData({
    //bounds: polyline.getBoundingBox()
  //});
}


/**
 * Creates a series of H.map.Marker points from the route and adds them to the map.
 * @param {Object} route  A route as received from the H.service.RoutingService
 */
function addManueversToMap(route){
  var svgMarkup = '<svg width="18" height="18" ' +
    'xmlns="http://www.w3.org/2000/svg">' +
    '<circle cx="8" cy="8" r="8" ' +
      'fill="#1b468d" stroke="white" stroke-width="1"  />' +
    '</svg>',
    dotIcon = new H.map.Icon(svgMarkup, {anchor: {x:8, y:8}}),
    group = new  H.map.Group(),
    i,
    j;

  // Add a marker for each maneuver
  for (i = 0;  i < route.leg.length; i += 1) {
    for (j = 0;  j < route.leg[i].maneuver.length; j += 1) {
      // Get the next maneuver.
      maneuver = route.leg[i].maneuver[j];
      // Add a marker to the maneuvers group
      var marker =  new H.map.Marker({
        lat: maneuver.position.latitude,
        lng: maneuver.position.longitude},
        {icon: dotIcon});
      marker.instruction = maneuver.instruction;
      group.addObject(marker);
    }
  }

  group.addEventListener('tap', function (evt) {
    map.setCenter(evt.target.getGeometry());
    openBubble(
       evt.target.getGeometry(), evt.target.instruction);
  }, false);

  // Add the maneuvers group to the map
  map.addObject(group);
}


/**
 * Creates a series of H.map.Marker points from the route and adds them to the map.
 * @param {Object} route  A route as received from the H.service.RoutingService
 */
function addWaypointsToPanel(waypoints){



  var nodeH3 = document.createElement('h3'),
    waypointLabels = [],
    i;
  nodeH3.style.marginLeft ='5%';
  nodeH3.style.marginRight ='5%';


   for (i = 0;  i < waypoints.length; i += 1) {
    waypointLabels.push(waypoints[i].label)
   }

   nodeH3.textContent = waypointLabels.join(' - ');

  //routeInstructionsContainer.innerHTML = '';
  routeInstructionsContainer.appendChild(nodeH3);
}

/**
 * Creates a series of H.map.Marker points from the route and adds them to the map.
 * @param {Object} route  A route as received from the H.service.RoutingService
 */
function addSummaryToPanel(summary){
	const hh = Math.floor(summary.travelTime/3600);
	const mm = Math.floor(summary.travelTime/60)%60;
  var summaryDiv = document.createElement('div'),
   content = '';
   content += '<b>Расстояние</b>: ' + summary.distance  + ' метров <br/>';
   content += '<b>Итого времени</b>: ' + hh + ' часов ' + mm + ' минут'; 
   console.log(summary.travelTime);


  summaryDiv.style.fontSize = 'small';
  summaryDiv.style.marginLeft ='5%';
  summaryDiv.style.marginRight ='5%';
  summaryDiv.innerHTML = content;
  routeInstructionsContainer.appendChild(summaryDiv);
}

/**
 * Creates a series of H.map.Marker points from the route and adds them to the map.
 * @param {Object} route  A route as received from the H.service.RoutingService
 */
function addManueversToPanel(route){



  var nodeOL = document.createElement('ol'),
    i,
    j;

  nodeOL.style.fontSize = 'small';
  nodeOL.style.marginLeft ='5%';
  nodeOL.style.marginRight ='5%';
  nodeOL.className = 'directions';

     // Add a marker for each maneuver
  for (i = 0;  i < route.leg.length; i += 1) {
    for (j = 0;  j < route.leg[i].maneuver.length; j += 1) {
      // Get the next maneuver.
      maneuver = route.leg[i].maneuver[j];

      var li = document.createElement('li'),
        spanArrow = document.createElement('span'),
        spanInstruction = document.createElement('span');

      spanArrow.className = 'arrow '  + maneuver.action;
      spanInstruction.innerHTML = maneuver.instruction;
      li.appendChild(spanArrow);
      li.appendChild(spanInstruction);

      nodeOL.appendChild(li);
    }
  }

  routeInstructionsContainer.appendChild(nodeOL);
}


