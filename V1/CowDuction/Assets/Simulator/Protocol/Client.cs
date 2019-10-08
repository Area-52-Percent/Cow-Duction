using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;

public class Client {
    private UdpClient client;
    public Packet packet;

    public Client(int localPort, string destIP, int destPort) {
        client = new UdpClient(localPort);
        client.Connect(destIP, destPort);
        packet = new Packet();
    }

    public Client() : this(4001, "10.10.101.91", 4001) { }

    public void sendData() {
        packet.tick();
        byte[] message = packet.getBytes();
        client.Send(message, message.Length);
    }

    public void close() {
        client.Close();
    }
}
