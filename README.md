# TOTS_Calendar.Events.API

API to manage events in the outlook calendar.

## Technologies

- Net Core 6
- Microsoft Graph
- MediatR
- FluentValidation

## Console
Console with the API logs.
![Alt text](Img/Captura_Console.png?raw=true  "Console")


## Authorize
Go to Authorize, select the entire scope and press authorize.

![Alt text](Img/Captura_OAuth.png?raw=true  "authorize")

![Alt text](Img/Captura_Login.png?raw=true  "Login")

![Alt text](Img/Captura_LoginOAuth.png?raw=true  "authorizete")


## Endpoints

![Alt text](Img/Captura_Swagger.png?raw=true  "swagger")

### Authorization

- GET
/api/v1/Authorization
(Autorizaciones de permisos para utilizar la api)


### Events

- GET
/api/v1/Calendar/Events
(Obtener los eventos)
![Alt text](Img/Captura_GetEvents.png?raw=true  "swagger")

- POST
/api/v1/Calendar/Events
(Creación de nueva eventos)
![Alt text](Img/Captura_PostEvents_Body.png?raw=true  "Body")

![Alt text](Img/CapturaResponseTypes.png?raw=true  "Responses")

- PATCH
/api/v1/Calendar/Events/{id}
(Actualización de eventos)


- DELETE
/api/v1/Calendar/Events/{id}
(Eliminación de eventos)

