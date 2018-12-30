# Climate comparison

Climate comparison is a project to compare climates of two chosen places on Earth.

You can see the online demo [here](https://climate-front.azurewebsites.net/).

It uses the temperature data from [Berkeley Earth](http://berkeleyearth.org), the precipitation data from [Climatic Research Unit, University of East Anglia](http://www.cru.uea.ac.uk/), and the cities data from [GeoNames](http://download.geonames.org/export/dump/).

Climate comparison runs on .NET Core, Sql Server and React.

## Getting the data
The temperature data is available freely from http://berkeleyearth.org/data/. Download a set of TMAX data, for example, Breakpoint Adjusted Monthly Station data, and extract CSVs.

The precipitation data is available from http://www.cru.uea.ac.uk/data/. Download cru_ts4.02.2011.2017.pre.dat.gz from https://crudata.uea.ac.uk/cru/data/hrg/cru_ts_4.02/cruts.1811131722.v4.02/pre/.

The cities data is available from http://download.geonames.org/export/dump/.  Download cities1000.zip and extract CSV.

## Importing data into the database
Create the database tables using the scripts from the `sql` directory.

Build all the solutions from the `tools` directory.

Import data by running the following commands:

```
cd tools\import
dotnet run --project cities\import-cities\import-cities <path to cities1000.txt> <sql server connection string>
dotnet run --project stations\import-stations\import-stations <path to site_detail.txt> <sql server connection string>
dotnet run --project tmax\import-tmax\import-tmax <path to data.txt> <sql server connection string>
dotnet run --project precipitation\import-precipitation\import-precipitation <path to data file> <starting year> <columns count> <rows count> <sql server connection string>
```
## Running everything
```
run.cmd
```

## Running backend
Have a connection string in the client secrets.
```
dotnet run --project backend\ClimateComparison
```

## Running frontend
```
cd frontend
npm update
npm start
```
