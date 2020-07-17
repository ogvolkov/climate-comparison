package org.climatecomparison.radiationimport;

import com.microsoft.azure.storage.StorageException;
import com.microsoft.azure.storage.table.CloudTable;
import com.microsoft.azure.storage.table.TableOperation;
import ucar.ma2.InvalidRangeException;

import java.io.BufferedReader;
import java.io.FileReader;
import java.io.IOException;

public class Importer {
    private Cdf cdf;

    private CloudTable table;

    public Importer(Cdf cdf, CloudTable table) {
        this.cdf = cdf;
        this.table = table;
    }

    public void run(String placesFileName, int from, int to) throws IOException, InvalidRangeException, StorageException {
        try (BufferedReader placesFile = new BufferedReader(new FileReader(placesFileName))) {
            String line;
            while ((line = placesFile.readLine()) != null) {
                String[] fields = line.split("\\s+");

                int placeId = Integer.parseInt(fields[0]);
                double lat = Double.parseDouble(fields[1]);
                double lon = Double.parseDouble(fields[2]);

                if (placeId < from) continue;
                if (placeId > to) break;

                double[] result = cdf.get(lat, lon);

                for (int i = 0; i < 12; i++) {
                    save(placeId, i + 1, result[i]);
                }

                System.out.println("Imported " + placeId);
            }
        }
    }

    private void save(int placeId, int month, double radiation) throws StorageException {
        RadiationEntity radiationRecord = new RadiationEntity(placeId, month, radiation);
        TableOperation replaceOperation = TableOperation.insertOrReplace(radiationRecord);
        table.execute(replaceOperation);
    }
}
