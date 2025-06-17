# How to run this
- Must have .NET 9 SDK installed.
- Configure WorkOS ClientId, WorkOS API Key and AuthKit Url in appsettings.json.
- Open terminal go to solution folder and execute 'dotnet run' command.
- Go to https://localhost:7150/swagger/index.html
- Click authorize and insert a WorkOS Bearer JWT Token in the Bearer
- Click execute in the GET WeatherForecast sample endpoint.
- If it works it should return a 200 with some data.

# What we want to do

- Do the same flow as above, but instead of inserting the WorkOS Bearer JWT Token, click authorize under the WorkOSAuthKit OAUth2 Authorization with PKCE and have it redirect us to AuthKit, then you log in and AuthKit redirects us back as a logged user.
- Currently if we try to do that we get this message in swagger after logging in to AuthKit: "auth warningAuthorization may be unsafe, passed state was changed in server. The passed state wasn't returned from auth server."
