using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Packet {
    private byte[] header = new byte[] { 0x00, 0x02, 0x60, 0x00, 0x00, 0x00, 0x00, 0x00 };
    private byte[] fsTimer = new byte[] { 0x00, 0x00, 0x00, 0xC0, 0x47, 0x61, 0x24, 0x40 };
    public int timeTick;
    public byte[] tmp = new byte[] { 0x00, 0x00, 0x00, 0x00 };
    public double xLinVel = 0.0001;
    public double yLinVel = 0.0001;
    public double zLinVel = 0.0001;
    public double pitchAngVel = 0.0001;
    public double rollAngVel = 0.0001;
    public double pitchPos = 0.0001;
    public double rollPos = 0.0001;
    public double altitude = 0.0001;
    private byte[] garbage = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

    public void tick() {
        timeTick++;
    }

    public byte[] getBytes() {
        byte[] result = new byte[96];
        copy(header, result, 0);
        copy(fsTimer, result, 8);
        copyInt(timeTick, result, 16);
        copy(tmp, result, 20);
        copyDouble(xLinVel, result, 24);
        copyDouble(yLinVel, result, 32);
        copyDouble(zLinVel, result, 40);
        copyDouble(pitchAngVel, result, 48);
        copyDouble(rollAngVel, result, 56);
        copyDouble(pitchPos, result, 64);
        copyDouble(rollPos, result, 72);
        copyDouble(altitude, result, 80);
        copy(garbage, result, 88);

        return result;
    }

    private void copy(byte[] from, byte[] to, int toIndex) {
        Array.Copy(from, 0, to, toIndex, from.Length);
    }

    private void copyInt(int i, byte[] to, int toIndex) {
		byte[] from = BitConverter.GetBytes(i);
		Array.Reverse(from);
        copy(from, to, toIndex);
    }

    private void copyDouble(double d, byte[] to, int toIndex) {
        copy(BitConverter.GetBytes(d), to, toIndex);
    }
}
