CREATE NONCLUSTERED INDEX Idx_Cities_Name_Population_CountryCode ON dbo.Cities (Name, Population DESC, CountryCode);

CREATE SPATIAL INDEX Idx_Stations_Location
ON dbo.Stations(Location)
USING GEOGRAPHY_GRID  
WITH (  
	GRIDS = (HIGH, HIGH, HIGH, HIGH ),  
	CELLS_PER_OBJECT = 512);
	
CREATE INDEX Idx_AverageHigh_Year_Month ON dbo.AverageHigh(Year, Month)