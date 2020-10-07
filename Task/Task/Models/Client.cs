using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation.Peers;

namespace Task.Models
{
    public class Client
    {
        public class Document {
            public String serial;
            public String number;
            public String date;
            public String expirationDate;

            public Document(String serial, String number, String date, String expirationDate) {
                this.serial = serial;
                this.number = number;
                this.date = date;
                this.expirationDate = expirationDate;
            }
        }

        public String name;
        public int age;
        public String registrationAddress;
        public String residenceAddress;
        public String phone;
        public String email;
        public List<Document> documents;

        public Client(String name, int age, String registrationAddress, String residenceAddress, String phone, String email, List<Document> docs=null) {
            this.name = name;
            this.age = age;
            this.registrationAddress = registrationAddress;
            this.residenceAddress = residenceAddress;
            this.documents = docs;
        }

        public void AddDocs(List<Document> docs) {
            this.documents = docs;
        }

        public void AddAddresses(String reg, String res) {
            this.registrationAddress = reg;
            this.residenceAddress = res;
        }
    }
}
