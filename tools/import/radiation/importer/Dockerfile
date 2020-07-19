FROM openjdk:11
COPY ./data /usr/app
COPY ./target/radiation-import-1.0-SNAPSHOT-jar-with-dependencies.jar /usr/app/
WORKDIR /usr/app
ENTRYPOINT ["java", "-jar", "radiation-import-1.0-SNAPSHOT-jar-with-dependencies.jar", "m.nc", "city_id_coords.txt"]