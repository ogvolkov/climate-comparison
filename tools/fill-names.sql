INSERT INTO CityNames(CityId, Name)
SELECT C.Id, N.value
FROM Cities C
CROSS APPLY STRING_SPLIT(AltNames, ',') N
WHERE N.value <> ''