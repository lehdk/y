# Y

## Backend configuration

You will need to add this to the configuration in order to run the backend api
```json
"ConnectionStrings": {
    "Database": "Data Source=IP_ADDRESS,PORT;Database=Y;Integrated Security=false;User ID=SA;Password=ExamplePassword;Encrypt=false;"
},
"Jwt": {
    "Issuer": "EXAMPLE ISSUER",
    "Audience": "EXAMPLE AUDIENCE",
    "TokenKey": "512_BUT_LONG_TOKEN_KEY"
},
```