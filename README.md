# WikiDown

Wiki-engine with articles written in Markdown.

Built on top of the following technologies:

- RavenDB
- ASP.NET MVC
- MarkdownSharp
- PageDown

# Getting started

- Install RavenDB on your computer
- Create a RavenDB-database and point to it in the `web.config`
- Set the password for the root-account in the `web.config`. This can be removed after the first successful run of the application, when the `root`-account has been created with given password.
- Make sure Nuget has restored all referenced packages
- Run the `WikiDown.Website`-project