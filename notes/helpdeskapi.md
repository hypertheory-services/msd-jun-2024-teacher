# Help Desk API

## User Story: Employees can report Issues for Software that is Supported by the Company

Operation: CreateIssueForSoftware

Operands:
    - Employee - OAUTH/OpenId Connect -> 401 
    - Supported Piece of Software
    - Issue 
        - Description: Description of your issue.



### Request:

Employee - Authorization Header in the Request

Software - Route Parameter. 

POST /catalog/{catalogItemId:guid}/issues



Issue:


```json

{
    "description": "Thing is busted!",
}

```


## Response

```json
{
  "id": "d27a8953-406e-494e-afc2-4b3aac5dd8d1",
  "description": "Want Clippy! Not Copilot!",
  "status": "Submitted",
  
  "_links": [
    {
      "rel": "support",
      "href": "/catalog/c26f955c-2c2c-46b9-a4a1-32c09e3b654a/issues/d27a8953-406e-494e-afc2-4b3aac5dd8d1/support"
    },
    {
      "rel": "user",
      "href": "/users/1ddc18f9-b4dc-468d-9828-279068e02c73"
    }
  ],
  "_embedded": {
    "data": {
      "id": "c26f955c-2c2c-46b9-a4a1-32c09e3b654a",
      "title": "Visual Studio 2022",
      "description": "An Integrated Development Environment for .NET Developer on Windows"
    },
    "_links": [
      {
        "rel": "self",
        "href": "/catalog/c26f955c-2c2c-46b9-a4a1-32c09e3b654a"
      }
    ]
  }
}
```
