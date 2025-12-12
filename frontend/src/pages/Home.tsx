import { Link } from 'react-router-dom';

export default function Home() {
  return (
    <div className="home-container">
      <div className="hero">
        <h1>Payroll Management System</h1>
        <p className="subtitle">Complete employee management and payroll calculations</p>
      </div>

      <div className="features-grid">
        <Link to="/create" className="feature-card">
          <div className="feature-icon">➕</div>
          <h2>Create Employee</h2>
          <p>Register new employees with automatic payroll calculation</p>
        </Link>

        <Link to="/search" className="feature-card">
          <div className="feature-icon">🔍</div>
          <h2>Search Employee</h2>
          <p>Find employees by ID or employee number quickly</p>
        </Link>

        <Link to="/list" className="feature-card">
          <div className="feature-icon">📋</div>
          <h2>List Employees</h2>
          <p>View all registered employees with edit and delete options</p>
        </Link>
      </div>

      <div className="info-section">
        <h2>Features</h2>
        <div className="info-grid">
          <div className="info-item">
            <h3>✅ CNPJ Validation</h3>
            <p>Complete Brazilian tax ID validation algorithm</p>
          </div>
          <div className="info-item">
            <h3>💰 Automatic Calculations</h3>
            <p>Social Security, Income Tax, FGTS, Overtime, Weekly Rest</p>
          </div>
          <div className="info-item">
            <h3>👨‍👩‍👧‍👦 Family Allowance</h3>
            <p>Automatic calculation by salary range</p>
          </div>
          <div className="info-item">
            <h3>🚌 Transportation Voucher</h3>
            <p>Optional, 6% of base salary</p>
          </div>
        </div>
      </div>

      <div className="stats-section">
        <div className="stat-card">
          <div className="stat-number">100%</div>
          <div className="stat-label">Automated Calculations</div>
        </div>
        <div className="stat-card">
          <div className="stat-number">4</div>
          <div className="stat-label">Core Features</div>
        </div>
        <div className="stat-card">
          <div className="stat-number">7</div>
          <div className="stat-label">REST API Endpoints</div>
        </div>
      </div>
    </div>
  );
}