# EPiServer Find optimizations export/import plugin
[![Build status](https://ci.appveyor.com/api/projects/status/9higbqvart6v07un/branch/master?svg=true)](https://ci.appveyor.com/project/SergVro/findexportimport/branch/master)

This is EPiServer CMS Admin mode plugin for EPiServer Find optimizations export / import.
The plugin allows to export and import synonyms, related queries, autocompletes and best bets.
More information is available in [this blog post](http://world.episerver.com/blogs/Sergey-Vorushilo/Dates/2015/11/find-optimizations-export--import/).

## Setup
The solution contains three projects:
* Vro.FindExportImport - this is API for EPiServer Find optimizations export and import
* Vro.FindExportImport.AdminPlugin - this is Admin mode plugin that provides UI for export/import
* TestSite - is a development EPiServer CMS site for plugin testing

In order to start with development do the following:

1. Ensure the restore NuGet packages option is on and build the solution. 
2. Create a free developer index in EPiServer Find on [find.episerver.com](http://find.episerver.com) and specify it in TestSite [web.config](https://github.com/SergVro/FindExportImport/blob/master/TestSite/Web.config#L18)
3. Run the TestSite project, navigate to Admin mode and open Find Export/Import plugin in Admin > Tools section.

The plugin is automatically installed on the test site everytime when the AdminPluign project is built. 

In order to create a NuGet package run `nuget pack` command inside the root folder of Vro.FindExportImport.AdminPlugin project.
