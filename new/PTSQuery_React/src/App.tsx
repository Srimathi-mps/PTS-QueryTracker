import React from 'react';
import { BrowserRouter as Router, Routes, Route, Link, useLocation } from 'react-router-dom';
import { 
  LayoutDashboard, 
  PlusCircle, 
  ClipboardList, 
  Settings, 
  Search, 
  Bell, 
  User,
  LogOut,
  ChevronRight
} from 'lucide-react';

import CommunicationTracker from './components/CommunicationTracker';
import DueForClosing from './components/DueForClosing';
import Configuration from './components/Configuration';

const SidebarLink = ({ to, icon: Icon, label }: { to: string, icon: any, label: string }) => {
  const location = useLocation();
  const isActive = location.pathname === to;
  
  return (
    <Link 
      to={to} 
      className={`flex items-center gap-4 px-4 py-3 rounded-xl transition-all ${
        isActive 
          ? 'bg-blue-600 text-white shadow-lg shadow-blue-600/20' 
          : 'text-slate-600 hover:bg-white hover:text-blue-600'
      }`}
    >
      <Icon size={20} />
      <span className="font-medium">{label}</span>
    </Link>
  );
};

const App: React.FC = () => {
  return (
    <Router>
      <div className="flex min-h-screen bg-[#e3e7ef] text-slate-800">
        {/* Sidebar */}
        <aside className="w-72 flex flex-col p-6 bg-slate-200/50 backdrop-blur-xl border-r border-slate-300">
          <div className="flex items-center gap-3 px-2 mb-10 mt-2">
            <div className="w-12 h-12 bg-gradient-to-tr from-blue-600 to-indigo-600 rounded-2xl flex items-center justify-center shadow-xl">
              <ClipboardList className="text-white w-7 h-7" />
            </div>
            <span className="text-2xl font-black tracking-tight text-slate-900">PTS Query</span>
          </div>
          
          <nav className="flex-1 space-y-2">
            <SidebarLink to="/" icon={LayoutDashboard} label="Comm Tracker" />
            <SidebarLink to="/raise" icon={PlusCircle} label="Raise Problem" />
            <SidebarLink to="/due" icon={ClipboardList} label="Due For Closing" />
            <SidebarLink to="/config" icon={Settings} label="Configuration" />
          </nav>
          
          <div className="mt-auto border-t border-slate-300 pt-6 px-2">
            <div className="flex items-center gap-3 mb-6 p-3 bg-white/50 rounded-2xl border border-white/50">
              <div className="w-10 h-10 bg-blue-600/10 rounded-full flex items-center justify-center border border-blue-600/20">
                <User size={20} className="text-blue-600" />
              </div>
              <div>
                <p className="text-sm font-bold text-slate-900">MAC014393</p>
                <p className="text-xs text-slate-500 font-medium tracking-wide uppercase">Administrator</p>
              </div>
            </div>
            <button className="flex items-center gap-3 w-full px-4 py-3 text-slate-600 hover:text-rose-600 hover:bg-rose-50 rounded-xl transition-all">
              <LogOut size={18} />
              <span className="text-sm font-bold">Sign Out</span>
            </button>
          </div>
        </aside>

        {/* Main Content */}
        <main className="flex-1 overflow-auto">
          {/* Header */}
          <header className="h-24 flex items-center justify-between px-10 sticky top-0 z-10">
            <div className="flex items-center gap-4 bg-white border border-slate-200 px-6 py-3 rounded-2xl w-[450px] shadow-sm">
              <Search className="text-slate-400" size={20} />
              <input 
                type="text" 
                placeholder="Search anything..." 
                className="bg-transparent border-none text-sm text-slate-900 focus:outline-none w-full font-medium"
              />
            </div>
            
            <div className="flex items-center gap-4">
              <button className="relative w-12 h-12 bg-white rounded-2xl flex items-center justify-center border border-slate-200 hover:border-blue-300 hover:bg-slate-50 transition-all shadow-sm">
                <Bell size={20} className="text-slate-600" />
                <span className="absolute top-3 right-3 w-2.5 h-2.5 bg-blue-500 rounded-full border-2 border-white"></span>
              </button>
              <div className="w-12 h-12 bg-white rounded-2xl flex items-center justify-center border border-slate-200 shadow-sm cursor-pointer hover:border-blue-300 transition-all">
                <Settings size={20} className="text-slate-600" />
              </div>
            </div>
          </header>

          {/* Routes */}
          <div className="max-w-7xl mx-auto px-10 pb-10">
            <Routes>
              <Route path="/" element={<CommunicationTracker />} />
              <Route path="/raise" element={<CommunicationTracker />} />
              <Route path="/due" element={<DueForClosing />} />
              <Route path="/config" element={<Configuration />} />
            </Routes>
          </div>
        </main>
      </div>
    </Router>
  );
};

export default App;
