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

        [XmlRpcMethod("modem.set_by_name")]
        String SetModem(string message);

        [XmlRpcMethod("modem.get_names")]
        String[] GetModems();
    }
}