using CookComputing.XmlRpc;

namespace XMLRPC
{
    [XmlRpcUrl("http://127.0.0.1:7362")]
    public interface IFldigiRPC : IXmlRpcProxy
    {
        [XmlRpcMethod("text.add_tx")]
        String AddText(string message);

        [XmlRpcMethod("main.tx")]
        String StartTx();

        [XmlRpcMethod("main.get_trx_state")]
        String GetTRState();

        [XmlRpcMethod("text.get_rx")]
        byte[] GetRXWidget(int start, int end);

        [XmlRpcMethod("text.get_rx_length")]
        int GetRXLength();

        [XmlRpcMethod("text.clear_rx")]
        String ClearRX();
    }
}