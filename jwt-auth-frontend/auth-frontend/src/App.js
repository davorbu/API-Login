import React, { useState } from "react";
import axios from "axios";
import { jwtDecode } from "jwt-decode";

function App() {
  const [token, setToken] = useState("");
  const [user, setUser] = useState({});
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");

  // Obrada login forme
  const handleLogin = async (e) => {
    e.preventDefault();
    try {
      const { data } = await axios.post(
        "http://localhost:5283/api/Auth/login",
        {
          email,
          password,
        }
      );
      setToken(data.token);
      const decoded = jwtDecode(data.token);
      setUser(decoded);
    } catch (error) {
      console.error("Authentikacija neuspješna", error);
      alert("Neispravni email ili lozinka.");
    }
  };

  // Funkcija za ažuriranje korisnika
  const handleUpdateUser = async () => {
    try {
      const { data } = await axios.post(
        "http://localhost:5283/api/Auth/update-user",
        {
          email: user.email, // Pretpostavka da ažuriramo trenutnog korisnika
          name: "NovoIme", // Ovdje trebate dodati logiku za unos novog imena
          role: "NoviRole", // Isto tako za unos nove uloge
        },
        {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        }
      );
      alert("Korisnik ažuriran!");
      setToken(data.token); // Postavite novi token ako se vrati
      setUser(jwtDecode(data.token)); // Ažurirajte korisničke podatke
    } catch (error) {
      console.error("Ažuriranje korisnika neuspješno", error);
      alert("Neuspjelo ažuriranje korisnika.");
    }
  };

  // Funkcija za testiranje dummy endpointa
  const handleDummyTest = async () => {
    try {
      await axios.post(
        "http://localhost:5283/api/Auth/dummy-test",
        {},
        {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        }
      );
      alert("Dummy test uspješan!");
    } catch (error) {
      console.error("Dummy test neuspješan", error);
      alert("Dummy test neuspješan.");
    }
  };

  return (
    <div>
      <h1>JWT Auth Demo</h1>
      <form onSubmit={handleLogin}>
        <div>
          <input
            type="email"
            placeholder="Email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            required
          />
        </div>
        <div>
          <input
            type="password"
            placeholder="Password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            required
          />
        </div>
        <button type="submit">Login</button>
      </form>

      {token && (
        <div>
          <h3>Informacije o token:</h3>
          <p>Valid do: {new Date(user.exp * 1000).toLocaleString()}</p>
          <p>Email: {user.email}</p>
          <p>Ime: {user.name}</p>
          <p>Uloga: {user.role}</p>
        </div>
      )}

      <button onClick={handleUpdateUser}>Ažuriraj korisnika</button>
      <button onClick={handleDummyTest}>Dummy Test</button>
    </div>
  );
}

export default App;
