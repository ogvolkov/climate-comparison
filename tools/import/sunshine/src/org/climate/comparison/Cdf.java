package org.climate.comparison;

import ucar.ma2.ArrayShort;
import ucar.ma2.InvalidRangeException;
import ucar.nc2.NetcdfFile;
import ucar.nc2.Variable;

import java.io.IOException;

public class Cdf implements AutoCloseable {
    private NetcdfFile ncfile;
    private Variable radiation;
    private double scaleFactor;
    private double offset;

    public Cdf(String filename) throws IOException {
        ncfile = NetcdfFile.open(filename);

        radiation = ncfile.findVariable("ssrd");
        scaleFactor = radiation.findAttribute("scale_factor").getNumericValue().doubleValue();
        offset = radiation.findAttribute("add_offset").getNumericValue().doubleValue();
    }

    public double[] get(int a, int b) throws IOException, InvalidRangeException {
        double[] result = new double[12];

        int[] origin = new int [] { 0, a, b };
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
