import { BrowserRouter as Router, Routes, Route, Link } from 'react-router-dom';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import Home from './pages/Home';
import CreateEmployee from './pages/CreateEmployee';
import SearchEmployee from './pages/SearchEmployee';
import EmployeeList from './pages/EmployeeList';
import './App.css';

const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      refetchOnWindowFocus: false,
      retry: 1,
    },
  },
});

function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <Router>
        <div className="app">
          <nav className="navbar">
            <div className="nav-container">
              <Link to="/" className="nav-logo">
                💼 Payroll Management System
              </Link>
              <ul className="nav-menu">
                <li className="nav-item">
                  <Link to="/" className="nav-link">Home</Link>
                </li>
                <li className="nav-item">
                  <Link to="/create" className="nav-link">Create</Link>
                </li>
                <li className="nav-item">
                  <Link to="/search" className="nav-link">Search</Link>
                </li>
                <li className="nav-item">
                  <Link to="/list" className="nav-link">List</Link>
                </li>
              </ul>
            </div>
          </nav>

          <main className="main-content">
            <Routes>
              <Route path="/" element={<Home />} />
              <Route path="/create" element={<CreateEmployee />} />
              <Route path="/search" element={<SearchEmployee />} />
              <Route path="/list" element={<EmployeeList />} />
              <Route path="/edit/:id" element={<CreateEmployee />} />
            </Routes>
          </main>

          <footer className="footer">
            <p>© 2024 Payroll Management System - All rights reserved</p>
          </footer>
        </div>
      </Router>
    </QueryClientProvider>
  );
}

export default App;