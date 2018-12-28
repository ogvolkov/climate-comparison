CREATE SPATIAL INDEX Idx_Stations_Location
ON dbo.Stations(Location)
USING GEOGRAPHY_GRID  
WITH (  
	GRIDS = (HIGH, HIGH, HIGH, HIGH ),  
	CELLS_PER_OBJECT = 512);
	
CREATE INDEX Idx_AverageHigh_Year_Month ON dbo.AverageHigh(Year, Month);

CREATE NONCLUSTERED INDEX Idx_CityNames_Name_CityId ON dbo.CityNames (Name, CityId);

CREATE SPATIAL INDEX Idx_Precipitation_Location
ON dbo.Precipitation(Location)
USING GEOGRAPHY_GRID  
WITH (  
	GRIDS = (HIGH, HIGH, HIGH, HIGH ),  
	CELLS_PER_OBJECT = 512);