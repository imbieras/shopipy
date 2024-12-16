// Navbar.jsx
import { Link, useLocation } from 'react-router-dom';

const Navbar = ({ onLogout }) => {
  const location = useLocation();
 
  const navItems = [
    { name: 'Services', path: '/services' },
    { name: 'Orders', path: '/orders' },
    { name: 'Appointments', path: '/appointments'}
  ];

  const isActive = (path) => {
    return location.pathname === path;
  };

  return (
    <header className="sticky top-0 z-50 w-full border-b border-border/40 bg-background/95 backdrop-blur supports-[backdrop-filter]:bg-background/60">
      <div className="flex h-16 items-center justify-between px-6 w-full">
        <div className="flex items-center gap-8">
          <Link to="/" className="flex items-center">
            <span className="text-xl font-bold tracking-tight">Shopipy</span>
          </Link>
          <nav className="hidden md:flex items-center gap-6">
            {navItems.map((item) => (
              <Link
                key={item.path}
                to={item.path}
                className={`text-sm font-medium transition-colors hover:text-primary ${
                  isActive(item.path)
                    ? 'text-foreground font-semibold'
                    : 'text-muted-foreground'
                }`}
              >
                {item.name}
              </Link>
            ))}
          </nav>
        </div>
        <button
          onClick={onLogout}
          className="inline-flex items-center justify-center px-4 py-2 text-sm font-medium text-destructive hover:text-destructive/90 hover:bg-destructive/10 rounded-md transition-colors"
        >
          Logout
        </button>
      </div>
    </header>
  );
};

export default Navbar;