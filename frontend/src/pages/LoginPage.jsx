import { useNavigate, useLocation } from "react-router-dom";
import { useAuth } from "../context/AuthContext.jsx";
import Card from "../components/ui/Card.jsx";
import Button from "../components/ui/Button.jsx";
import { useDocumentTitle } from "../hooks/useDocumentTitle.js";

// ── Slide 38 demo helper ─────────────────────────────────────────
// A deliberately fake login: pick a role, no password. The point of
// THIS lecture is what the frontend does with the logged-in user
// (guards, conditional UI, tokens on requests). Real authentication
// arrives with the ASP.NET Core backend in Lectures 03–04.
export default function LoginPage() {
  useDocumentTitle("Login");
  const { login } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();

  // If a guard redirected us here, go back there after login.
  const from = location.state?.from?.pathname || "/";

  const handleLogin = (role) => {
    login(role);
    navigate(from, { replace: true });
  };

  return (
    <Card className="card form-card">
      <h2>Demo Login</h2>
      <p className="muted">
        No password – this simulates the result of a successful login so we
        can teach route guards. Try: login as <b>student</b>, then visit
        /admin and watch the role guard redirect you.
      </p>
      <div style={{ display: "flex", gap: 10 }}>
        <Button onClick={() => handleLogin("student")}>
          Login as Student
        </Button>
        <Button variant="secondary" onClick={() => handleLogin("admin")}>
          Login as Canteen Admin
        </Button>
      </div>
    </Card>
  );
}
