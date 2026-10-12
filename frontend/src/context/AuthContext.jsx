import { createContext, useContext } from "react";
import { useLocalStorage } from "../hooks/useLocalStorage.js";

// ── Slides 38 & 45: Context for GLOBAL CLIENT STATE ──────────────
// Auth state is the textbook use case for Context: small, changes
// rarely, needed everywhere (navbar, route guards, API client).
// NOTE: this login is FAKE on purpose. In Lectures 03–04 we build the
// real ASP.NET Core + JWT version – the React side barely changes.
const AuthContext = createContext(null);

const DEMO_USERS = {
  student: { name: "Kasun Perera", studentId: "IT22045678", role: "student" },
  admin: { name: "Canteen Admin", studentId: "STAFF001", role: "admin" },
};

export function AuthProvider({ children }) {
  const [user, setUser] = useLocalStorage("campuseats-user", null);

  const login = (roleKey) => setUser(DEMO_USERS[roleKey]);
  const logout = () => setUser(null);

  return (
    <AuthContext.Provider value={{ user, login, logout }}>
      {children}
    </AuthContext.Provider>
  );
}

// Custom hook so consumers write useAuth() instead of useContext(AuthContext)
export function useAuth() {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error("useAuth must be used inside <AuthProvider>");
  return ctx;
}
