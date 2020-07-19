import ucar.ma2.ArrayShort;
import ucar.ma2.InvalidRangeException;
import ucar.nc2.NetcdfFile;
import ucar.nc2.Variable;

import java.io.IOException;

public class Cdf implements AutoCloseable {
    private NetcdfFile ncfile;
    private Variable radiation;

    private long longitudeGranularity;
    private long latitudeGranularity;

    private double scaleFactor;
    private double offset;

    public Cdf(String filename) throws IOException {
        ncfile = NetcdfFile.open(filename);

        Variable longitude = ncfile.findVariable("longitude");
        longitudeGranularity = longitude.getSize();

        Variable latitude = ncfile.findVariable("latitude");
        latitudeGranularity = latitude.getSize();

        radiation = ncfile.findVariable("ssrd");
        scaleFactor = radiation.findAttribute("scale_factor").getNumericValue().doubleValue();
        offset = radiation.findAttribute("add_offset").getNumericValue().doubleValue();
    }

    // lat: [-90, 90]
    // lon: (-180, 180]
    public double[] get(double lat, double lon) throws IOException, InvalidRangeException {
        double[] result = new double[12];

        int r = 1 + (int)Math.round((90 - lat) * (latitudeGranularity-1) / 180);
        if (lon < 0) lon += 360;
        int c = 1 + (int)Math.round(lon * (longitudeGranularity-1) / 360);

        int[] origin = new int [] { 0, r, c };
        int[] size = new int [] { 12, 1, 1 };

        ArrayShort.D1 data = (ArrayShort.D1)radiation.read(origin, size).reduce();
        for (int i = 0; i < 12; i++) {
            short s = data.get(i);
            result[i] = offset + s * scaleFactor;
        }

        return result;
    }

    public void close() throws IOException {
        ncfile.close();
    }
}
