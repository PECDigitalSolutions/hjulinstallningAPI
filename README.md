# hjulinstallningAPI
🚀 HjulinstallningAPI
A .NET Core API for vehicle data retrieval using D&B’s database, with caching, JWT authentication, and local MSSQL storage.

📌 Features
✅ D&B API Integration – Fetch vehicle data from Dun & Bradstreet

✅ Local Database (MSSQL) – Cache vehicle data for performance

✅ JWT Authentication – Secure API access with tokens

✅ WowCharacter API – Example entity for API structure testing

✅ Swagger UI – Auto-generated API documentation


🔧 Setup Instructions

1️⃣ Install Prerequisites

Ensure you have the following installed:

.NET 8 SDK
SQL Server Express (or another SQL instance)
Postman (for API testing)
2️⃣ Clone the Repository

git clone https://github.com/PECDigitalSolutions/hjulinstallningAPI.git

cd hjulinstallningAPI

3️⃣ Configure Your Database & API Keys

Update appsettings.json (or appsettings.Development.json) with:

"DnbApi": 
{
  "BaseUrl": "https://api.dnb.com/v1/vehicledata",
  "IdKey": "YOUR_ID_KEY",
  "Lk": "YOUR_LK",
  "KundId": "YOUR_KUND_ID",
  "Password": "YOUR_PASSWORD",
  "ProduktId": "YOUR_PRODUKT_ID"
},

"JwtSettings": 
{
  "SecretKey": "YOUR_SECRET_KEY",
  "Issuer": "https://localhost:5222",
  "Audience": "https://localhost:5222"
},

"ConnectionStrings": 
{
  "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=VehicleDb;Trusted_Connection=True;"
}

4️⃣ Apply Database Migrations
dotnet ef database update
This will create the necessary tables in your SQL Server.

5️⃣ Run the API
dotnet run
API will start on:
➡️ http://localhost:5222

🛠 API Endpoints
🟢 Authentication
Method	Endpoint	Description
POST	/api/auth/login	Logs in a user and returns a JWT token
📌 Example Request:

{
  "username": "admin",
  "password": "password"
}

🚗 Vehicle API
📌 Testing in Swagger
Swagger UI is available at:
➡️ http://localhost:5222/swagger

🛡 Security
JWT Authentication protects API endpoints
Store JWT in localStorage / sessionStorage on the frontend
Tokens expire after 1 hour for security
🛠 Future Improvements
✅ Implement Refresh Tokens for longer session management

✅ Enhance Error Handling for API failures

✅ Build a Frontend (React/Angular) for better UX

📜 License

This project is open-source under the MIT License.

📬 Contact
For any questions or issues, contact PEC Digital Solutions
📧 support@pecdigitalsolutions.com
GitHub: PECDigitalSolutions