import { useEffect, useState } from 'react';
import { apiBaseUrl, getHealth, getMenu } from './api';

const currency = (value) =>
  new Intl.NumberFormat('en-LK', {
    style: 'currency',
    currency: 'LKR',
    maximumFractionDigits: 0,
  }).format(value);

export default function App() {
  const [menu, setMenu] = useState([]);
  const [health, setHealth] = useState(null);
  const [error, setError] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    let cancelled = false;

    async function load() {
      setLoading(true);
      setError(null);
      try {
        const [items, status] = await Promise.all([getMenu(), getHealth()]);
        if (cancelled) return;
        setMenu(items);
        setHealth(status);
      } catch (err) {
        if (!cancelled) setError(err.message);
      } finally {
        if (!cancelled) setLoading(false);
      }
    }

    load();
    return () => {
      cancelled = true;
    };
  }, []);

  const categories = [...new Set(menu.map((item) => item.category))];

  return (
    <main className="page">
      <header className="hero">
        <p className="eyebrow">SE3090 &middot; Lab 11 &middot; DevOps &amp; Cloud Hosting</p>
        <h1>CampusEats</h1>
        <p className="tagline">
          Today&rsquo;s menu, served from a cloud-hosted ASP.NET Core API and a
          managed PostgreSQL database.
        </p>
        <dl className="wiring">
          <div>
            <dt>Frontend</dt>
            <dd>Vercel</dd>
          </div>
          <div>
            <dt>API (VITE_API_URL)</dt>
            <dd className="mono">{apiBaseUrl ?? 'not configured'}</dd>
          </div>
          <div>
            <dt>Database</dt>
            <dd>
              {health ? (
                <span className={`pill ${health.database === 'connected' ? 'ok' : 'bad'}`}>
                  {health.database}
                </span>
              ) : (
                <span className="pill">&hellip;</span>
              )}
            </dd>
          </div>
        </dl>
      </header>

      {loading && <p className="note">Loading the menu from the API&hellip;</p>}

      {error && (
        <div className="note error">
          <strong>Could not reach the API.</strong>
          <span className="mono">{error}</span>
          <span>
            Check that VITE_API_URL points at the live backend and that the
            backend&rsquo;s CORS policy allows this origin.
          </span>
        </div>
      )}

      {!loading && !error && (
        <section className="menu">
          {categories.map((category) => (
            <div key={category} className="category">
              <h2>{category}</h2>
              <ul>
                {menu
                  .filter((item) => item.category === category)
                  .map((item) => (
                    <li key={item.id} className={item.available ? '' : 'sold-out'}>
                      <span className="name">{item.name}</span>
                      <span className="price">{currency(item.price)}</span>
                      {!item.available && <span className="badge">sold out</span>}
                    </li>
                  ))}
              </ul>
            </div>
          ))}
        </section>
      )}

      <footer className="foot">
        <span>{menu.length} items loaded from /api/menu</span>
        {health && <span className="mono">api: {health.status}</span>}
      </footer>
    </main>
  );
}
