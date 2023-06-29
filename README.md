# JourneyDrawing

https://en.wikipedia.org/wiki/A*_search_algorithm#:~:text=A*%20is%20an%20informed%20search,shortest%20time%2C%20etc.).

http://theory.stanford.edu/~amitp/GameProgramming/AStarComparison.html


open street map api c#
	https://wiki.openstreetmap.org/wiki/Software_libraries
	https://wiki.openstreetmap.org/wiki/Using_OpenStreetMap_offline
	
	
mapbox



------------------------------

the map comes from here : https://fr.wikipedia.org/wiki/Transverse_universelle_de_Mercator
https://en.wikipedia.org/wiki/Universal_Transverse_Mercator_coordinate_system
https://en.wikipedia.org/wiki/Universal_Transverse_Mercator_coordinate_system#/media/File:Universal_Transverse_Mercator_zones.svg

How to plot a point on a map given the utm coordinates
	https://maptools.com/tutorials/utm/quick_guide


the geodesic coordinates are initially in WGS84 (GPS), then converted into UTM coordinates with a zone number

https://coordinates-converter.com/en/decimal/40.783060,-73.971249?karte=OpenStreetMap&zoom=8

When we have the UTM we can convert to pixels on a 2D flat map

--------------------
https://www.bel-horizon.eu/images/site/cartographie/notion/utm/MGRS2.pdf

Baltimore
	Easting : 361090.78366456285594525871493M --> 090
	Northing : 4350293.5462882115581620724220M --> 293
	S18
	
	map : 
		1848 width  842 height
		S18 : 
			position bottom-left : left=524, top=267
			position top-left : left=524, top=226
			height 267-226=41, width 554-524=30
	
	Easting on map : (The map has grid lines spaced every kilometer or 1000 meters)
		left : 524 + ((090/1000)*30)=527
		top : 226 + (((1000-293)/1000)*41)=


New York - carte wikipedia travaillée
	Easting : 583958.19576228856442165139368M --> 958
				583958.196 from https://coordinates-converter.com/en/decimal/40.712728,-74.006015?karte=OpenStreetMap&zoom=8
	Northing : 4507342.9915585880186022928583M --> 342
				4507342.991 from https://coordinates-converter.com/en/decimal/40.712728,-74.006015?karte=OpenStreetMap&zoom=8
	T18
			position top-left : left=524, top=185
			height 226-185=41, width 554-524=30
	
	Easting on map : (The map has grid lines spaced every kilometer or 1000 meters)
		left : 524 + ((958/1000)*30)=553
		top : 185 + (((1000-342)/1000)*41)=212
		
New York - carte wikipedia 2 bleue
	Easting : 583958.19576228856442165139368M --> 958
	Northing : 4507342.9915585880186022928583M --> 342
	T18
			position top-left : left=581, top=239
			height 285-239=46, width 615-581=34
	
	Easting on map : (The map has grid lines spaced every kilometer or 1000 meters)
		left : 581 + ((958/1000)*34)=614
		top : 239 + (((1000-342)/1000)*46)=269