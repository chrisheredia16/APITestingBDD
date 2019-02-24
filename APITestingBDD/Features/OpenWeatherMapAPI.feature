Feature: OpenWeatherMapAPI
	To test the OpenWeatherMapAPI functionality
	Here we will test the Get by city name,id

@SmokeTest
Scenario: Get weather by city name
	Given I want to know the weather in a city
	| city        |
	| Zaragoza,ES |
	When I pass the city to the OpenWeatherMap API
	Then The result should match with the city sent
