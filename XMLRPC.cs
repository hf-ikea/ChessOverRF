using CookComputing.XmlRpc;

namespace XMLRPC
{
    [XmlRpcUrl("http://127.0.0.1:7362")]
    public interface IFldigiRPC : IXmlRpcProxy
    {
        [XmlRpcMethod("text.add_tx")]
        string AddText(string message);

        [XmlRpcMethod("main.tx")]
        string StartTx();

        [XmlRpcMethod("main.get_trx_state")]
        string GetTRState();

        [XmlRpcMethod("text.get_rx")]
        byte[] GetRXWidget(int start, int end);

        [XmlRpcMethod("text.get_rx_length")]
        int GetRXLength();

        [XmlRpcMethod("text.clear_rx")]
        string ClearRX();
    }
}