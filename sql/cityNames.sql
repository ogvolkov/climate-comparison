CREATE TABLE CityNames(
	Id INT NOT NULL IDENTITY PRIMARY KEY,
	CityId INT NOT NULL REFERENCES Cities(Id),
	Name NVARCHAR(200) NOT NULL
);