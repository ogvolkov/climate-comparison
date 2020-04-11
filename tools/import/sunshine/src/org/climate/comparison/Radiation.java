package org.climate.comparison;

import com.microsoft.azure.storage.table.TableServiceEntity;

public class Radiation extends TableServiceEntity {
    private double radiation;

    public Radiation(int placeId, int month, double radiation) {
        this.partitionKey = Integer.toString(placeId);
        this.rowKey = String.format("%02d", month);
        this.radiation = radiation;
    }

    public double getRadiation() {
        return radiation;
    }

    public void setRadiation(double radiation) {
        this.radiation = radiation;
    }
}
