# Climate comparison

Climate comparison is a tiny project to compare climates of two chosen places on Earth. It uses climate data from [Berkeley Earth](http://berkeleyearth.org), cities data from [geonames](http://download.geonames.org/export/dump/), and runs on React, .NET Core and Sql Server.

## Getting the data
Climate data is available freely from http://berkeleyearth.org/data/. Download a set of TMAX data, for example, Breakpoint Adjusted Monthly Station data, and extract CSVs.

Cities data is available from http://download.geonames.org/export/dump/. 

Download  cities1000.zip and extract CSV.

## Importing data into the database
Create database tables using scripts from `sql` directory.

Build all tools solutions from `tools` directory.

Import data by running the following commands:

```
cd tools\import
dotnet run --project cities\import-cities\import-cities <path to cities1000.txt> <sql server connection string>
dotnet run --project stations\import-stations\import-stations <path to site_detail.txt> <sql server connection string>
dotnet run --project tmax\import-tmax\import-tmax <path to data.txt> <sql server connection string>
```

## Running backend
Have a connection string somehow in client secrets.
```
dotnet run --project backend\ClimateComparison
```

## Running frontend
```
cd frontend
npm update
npm start
```

## Usage
Start typing in the input, autocomplete will show what suitable places are available in the database. After selecting a place, the table with average daily high temperatures by each month will appear. If two places are selected, climates are roughly compared in text.
![UI screenshot](https://ogvolkov.github.io/images/climate-comparison.png)
