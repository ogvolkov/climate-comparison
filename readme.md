# Climate comparison

Climate comparison is a tiny project to compare climates of two chosen places on Earth. It uses the data from [Berkeley Earth](http://berkeleyearth.org), and runs on React, .NET Core and Sql Server.

## Getting the data
Data is available freely from http://berkeleyearth.org/data/. Download a set of TMAX data, for example, Breakpoint Adjusted Monthly Station data, and extract CSVs.

Import CSV into a Sql Server database using one to one mapping of site_detail text file to site_detail table and data file to tmax_data table.

Run scripts from
```
data\berkeley_earth\scripts.sql
```
to create and fill more properly named tables.

## Running backend
Have a connection string somehow in client secrets.
```
cd backend\ClimateComparison
dotnet run
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

## Comments
Berkeley Earth selection of places does not seem to be very suitable if you are interested in bigger cities (as opposed to more even surface coverage).