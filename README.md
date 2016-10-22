# SFWorkshopPlacement
This workshop is all about placing of services using placement constraints and different ways of communicating between the services. In particular, a frontend node type and a backend node type. The frontend is meant to simulate a DMZ while the backend is the actual service doing the work.

There is a frontend stateless service and a backend stateless service and they are deployed on the node types accordingly. The backend service has two endpoints listening on different ports at the same time. One endpoint is for the Web API (Port 25000) and the other endpoint is for Remoting (Port 25001). The default web page can be used to test both scenario and understand the difference between the two ways of communicating between services.

The top part of the web page is straight forward and simply doubles the number that is passed in. This flow is leveraging Remoting and the Service Fabric Naming Service (used for service discovery) to access the backend stateless service.

The bottom part of the web page is the more traditional way of accessing services on different systems. The number to be doubled textbox is straight forward. The other text box was added so the user can dynamically play with the URIs to test Web API directly or through Reverse Proxy. If you are not familiar with Reverse Proxy, click [here](https://azure.microsoft.com/en-us/documentation/articles/service-fabric-reverseproxy/) for more details.

API Examples:

Navigate to frontend node (eg. http://<SF Cluster Name>:1603/) will bring you to the default web page.

If you have the backend node exposed externally for testing purposes and Reverse Proxy configured then navigating to the following will call the API directly (eg. http://<SF Cluster Name>:19008/SFWorkshopPlacement/BackEndMathService/api/doubles/6)
