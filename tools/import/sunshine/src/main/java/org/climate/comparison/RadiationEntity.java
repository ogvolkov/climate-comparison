import com.microsoft.azure.storage.table.TableServiceEntity;

public class RadiationEntity extends TableServiceEntity {
    private double radiation;

    public RadiationEntity(int placeId, int month, double radiation) {
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
