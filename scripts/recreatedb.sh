#! /bin/sh
rm -R src/Anima.Data/Migrations/*
rm src/Anima.Api/anima.db
dotnet ef migrations add InitialCreate --project src/Anima.Data --startup-project src/Anima.Api
dotnet ef database update --project src/Anima.Data --startup-project src/Anima.Api