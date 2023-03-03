# Climate comparison

[![Build and Deploy](https://github.com/ogvolkov/climate-comparison/actions/workflows/ci_cd.yml/badge.svg)](https://github.com/ogvolkov/climate-comparison/actions/workflows/ci_cd.yml)

Climate comparison is a project to compare climates of two chosen places on Earth.

You can see the online demo [here](https://climatecomparisonprod.z6.web.core.windows.net/).

It uses the temperature data from [Berkeley Earth](http://berkeleyearth.org), the precipitation data from [Climatic Research Unit, University of East Anglia](http://www.cru.uea.ac.uk/), and the cities data from [GeoNames](http://download.geonames.org/export/dump/).

Climate comparison runs on .NET Core, Azure Table Storage and React.

## Getting the data
The temperature data is available freely from http://berkeleyearth.org/data/. Download a set of TMAX data, for example, Breakpoint Adjusted Monthly Station data, and extract CSVs.

The precipitation data is available from http://www.cru.uea.ac.uk/data/. Download cru_ts4.02.2011.2017.pre.dat.gz from https://crudata.uea.ac.uk/cru/data/hrg/cru_ts_4.02/cruts.1811131722.v4.02/pre/.

The cities data is available from http://download.geonames.org/export/dump/.  Download cities1000.zip and extract CSV.