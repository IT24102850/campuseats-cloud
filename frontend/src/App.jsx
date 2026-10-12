import { lazy, Suspense } from "react";
import { Routes, Route } from "react-router-dom";
import AppLayout from "./layouts/AppLayout.jsx";
import HomePage from "./pages/HomePage.jsx";
import LoginPage from "./pages/LoginPage.jsx";
import NotFoundPage from "./pages/NotFoundPage.jsx";
import MenuPage from "./features/menu/MenuPage.jsx";
import RegisterPage from "./features/register/RegisterPage.jsx";
import ProtectedRoute from "./routes/ProtectedRoute.jsx";
import RoleRoute from "./routes/RoleRoute.jsx";
import Spinner from "./components/ui/Spinner.jsx";

// ── Slide 37: LAZY ROUTES ────────────────────────────────────────
// These pages are split into separate JS chunks and only downloaded
// when the user first navigates there. Demo: open DevTools → Network,
// click "Admin" and watch a new chunk load.
const OrdersPage = lazy(() => import("./features/orders/OrdersPage.jsx"));
const AdminPage = lazy(() => import("./pages/AdminPage.jsx"));
const PerfDemoPage = lazy(() => import("./features/performance/PerfDemoPage.jsx"));

export default function App() {
  return (
    <Suspense fallback={<Spinner label="Loading page…" />}>
      <Routes>
        {/* ── Slide 37: LAYOUT ROUTE ─ AppLayout renders the navbar
             once; child pages render inside its <Outlet />. */}
        <Route element={<AppLayout />}>
          <Route path="/" element={<HomePage />} />
          <Route path="/menu" element={<MenuPage />} />
          <Route path="/register" element={<RegisterPage />} />
          <Route path="/login" element={<LoginPage />} />
          <Route path="/perf" element={<PerfDemoPage />} />

          {/* ── Slide 38: PROTECTED ROUTES ─ must be logged in */}
          <Route element={<ProtectedRoute />}>
            <Route path="/orders" element={<OrdersPage />} />

            {/* ── Slide 38: ROLE-BASED ROUTE ─ admins only */}
            <Route element={<RoleRoute allow={["admin"]} />}>
              <Route path="/admin" element={<AdminPage />} />
            </Route>
          </Route>

          {/* ── Slide 39: catch-all 404 error page */}
          <Route path="*" element={<NotFoundPage />} />
        </Route>
      </Routes>
    </Suspense>
  );
}
