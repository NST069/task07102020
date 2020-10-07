using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Task.Viewmodels
{
    class MainViewModel : INotifyPropertyChanged
    {
        private List<Models.Client> _clients;
        public List<Models.Client> Clients {
            get { return _clients; }
            set {
                _clients = value;
                OnPropertyChanged(nameof(Clients));
            }
        }

        public void Update() {
            String queryString = "SELECT client.name, client.age, reg.address, res.address, client.phone, client.email FROM clients " +
                "JOIN addresses AS reg ON reg.clientId=clients.id && reg.type=1 " +
                "JOIN addresses AS res ON res.clientId=clients.id && res.type=0 ";
            DataTable clients = Models.DBConnector.ExecuteQuery(queryString);
            Clients.Clear();
            foreach (DataRow x in clients.Rows) {
                Models.Client c = new Models.Client(x["Name"].ToString(), (int)x["Age"], x["RegistrationAddress"].ToString(), x["ResidenceAddress"].ToString(), x["Phone"].ToString(), x["Email"].ToString());
                DataTable documents = Models.DBConnector.ExecuteQuery("SELECT serial, number, date, expirationDate FROM documents WHERE clientId="+x["id"].ToString());
                List<Models.Client.Document> docs = new List<Models.Client.Document>();
                foreach (DataRow y in documents.Rows) {
                    docs.Add(new Models.Client.Document(y["Serial"].ToString(), y["Number"].ToString(), y["Date"].ToString(), y["ExpirationDate"].ToString()));
                }
                c.AddDocs(docs);
                Clients.Add(c);
            }
        }

        public async void UpdateAsync() {

            DataTable clients = await System.Threading.Tasks.Task.Run(() => Models.DBConnector.ExecuteQuery("SELECT * FROM clients"));
            Clients.Clear();
            foreach (DataRow x in clients.Rows) {
                int id = (int)x["id"];
                DataTable addresses = await System.Threading.Tasks.Task.Run(() => Models.DBConnector.ExecuteQuery("SELECT * FROM addresses WHERE clientId = "+id));
                
                DataTable documents = await System.Threading.Tasks.Task.Run(()=> Models.DBConnector.ExecuteQuery("SELECT * FROM documents WHERE clientId = " + id));
                List<Models.Client.Document> docs = new List<Models.Client.Document>();
                foreach (DataRow y in documents.Rows)
                {
                    docs.Add(new Models.Client.Document(y.Field<String>(1), y.Field<String>(2), y.Field<String>(3), y.Field<String>(4)));
                }
                Clients.Add(new Models.Client(x["Name"].ToString(), (int)x["Age"], addresses.Select("AddrType = 1")[0]["Address"].ToString(), addresses.Select("AddrType = 0")[0]["Address"].ToString(), x["Phone"].ToString(), x["Email"].ToString(), docs));
            }
        }


        public void SaveClient(Models.Client client) {//id, name, age, phone, email
            String queryString = "INSERT INTO clients OUTPUT INSERTED.ID VALUES (" + client.name + ", " + client.age + ", " + client.phone + ", " + client.email + ")";
            int id = (int)Models.DBConnector.ExecuteQuery(queryString).Rows[0]["ID"];
            SaveAddress(id, client.registrationAddress, "registration");
            SaveAddress(id, client.residenceAddress, "residence");
            SaveDocs(id, client.documents);
        }

        public void SaveDocs(int clientId, List<Models.Client.Document> documents) { //clientId, serial, number, date, expirationDate
            String queryString = "INSERT INTO documents VALUES ";
            foreach (var doc in documents) {
                queryString += "("+clientId+", "+doc.serial+","+doc.number+", "+doc.date+", "+doc.expirationDate+")";
            }

            Models.DBConnector.ExecuteQuery(queryString);
        }

        public void SaveAddress(int clientId, String address, String addrType) { //clientId, address, addrType(0-registration, 1-residence)
            String queryString = "INSERT INTO addresses VALUES(" + clientId + ", " + address +", "+((addrType=="registration")?1:0)+ ")";

            Models.DBConnector.ExecuteQuery(queryString);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] String info = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }
    }
}
