CREATE TABLE Precipitation(
	Id INT NOT NULL IDENTITY PRIMARY KEY,
	Location GEOGRAPHY NOT NULL,
	Year INT NOT NULL,
	Month INT NOT NULL,
	Precipitation FLOAT
)