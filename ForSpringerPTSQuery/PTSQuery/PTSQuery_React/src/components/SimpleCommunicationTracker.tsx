import React, { useState, useEffect } from 'react';
import { 
  Search, 
  RotateCcw, 
  MessageSquare, 
  ChevronDown,
  X,
  Inbox,
  Filter,
  Loader2
} from 'lucide-react';
import { ProblemService } from '../services/api';

const SimpleCommunicationTracker: React.FC = () => {
  const [loading, setLoading] = useState(false);
  const [problems, setProblems] = useState<any[]>([]);
  const [customers, setCustomers] = useState<any[]>([]);
  const [departments, setDepartments] = useState<any[]>([]);

  // Filter states
  const [filters, setFilters] = useState({
    customer: 'Hachette',
    project: 'Back-to-School 2026',
    job: 'J-77245',
    isbn: '978-0-06-093546-7',
    stage: 'Design',
    department: 'Editorial'
  });

  useEffect(() => {
    // Initial fetch if needed
    // fetchInitialData();
  }, []);

  return (
    <div className="min-h-screen bg-[#f8fafc] p-8 font-sans text-slate-900">
      <div className="max-w-7xl mx-auto space-y-6">
        
        {/* Header */}
        <div className="flex justify-between items-start">
          <div className="flex gap-4">
            <div className="w-12 h-12 bg-blue-600 rounded-xl flex items-center justify-center text-white shadow-lg shadow-blue-600/20">
              <MessageSquare size={24} fill="currentColor" fillOpacity={0.2} />
            </div>
            <div>
              <h1 className="text-2xl font-bold tracking-tight text-slate-800">Communication Tracker</h1>
              <p className="text-slate-500 text-sm mt-1">Filter customer communications across projects, jobs and production stages.</p>
            </div>
          </div>
          <div className="bg-white px-4 py-1.5 rounded-full border border-slate-200 shadow-sm">
            <span className="text-[13px] font-bold text-slate-700">15 total records</span>
          </div>
        </div>

        {/* Filters Card */}
        <div className="bg-white rounded-2xl border border-slate-200 shadow-sm overflow-hidden">
          <div className="p-6 border-b border-slate-100 flex justify-between items-center bg-slate-50/50">
            <div className="flex items-center gap-2">
              <Filter size={16} className="text-slate-400" />
              <h2 className="text-sm font-bold text-slate-700">Filters</h2>
            </div>
            <div className="bg-blue-50 text-blue-700 px-3 py-1 rounded-md text-[11px] font-black uppercase tracking-wider">
              6 active
            </div>
          </div>
          <div className="p-8 space-y-8">
            <p className="text-[13px] text-slate-400 font-medium">Choose any combination, then press Go.</p>
            
            <div className="grid grid-cols-1 md:grid-cols-3 gap-x-8 gap-y-6">
              {[
                { label: 'CUSTOMER', key: 'customer', value: filters.customer },
                { label: 'PROJECT', key: 'project', value: filters.project },
                { label: 'JOB', key: 'job', value: filters.job },
                { label: 'ISBN', key: 'isbn', value: filters.isbn },
                { label: 'STAGE', key: 'stage', value: filters.stage },
                { label: 'DEPARTMENT', key: 'department', value: filters.department },
              ].map((field) => (
                <div key={field.key} className="space-y-2">
                  <label className="text-[11px] font-bold text-slate-400 uppercase tracking-wider ml-1">{field.label}</label>
                  <div className="relative group">
                    <select 
                      className="w-full bg-white border border-slate-200 rounded-xl px-4 py-3 text-[14px] text-slate-700 font-semibold appearance-none focus:outline-none focus:ring-2 focus:ring-blue-500/20 focus:border-blue-500 transition-all cursor-pointer group-hover:border-slate-300"
                      value={field.value}
                      onChange={(e) => setFilters({...filters, [field.key]: e.target.value})}
                    >
                      <option>{field.value}</option>
                    </select>
                    <ChevronDown className="absolute right-4 top-1/2 -translate-y-1/2 text-slate-400 pointer-events-none group-hover:text-slate-500" size={16} />
                  </div>
                </div>
              ))}
            </div>

            <div className="flex items-center gap-3 pt-4">
              <button className="flex items-center gap-2 bg-blue-600 hover:bg-blue-700 text-white px-8 py-3 rounded-xl font-bold transition-all shadow-md shadow-blue-600/10 active:scale-95">
                <Search size={18} />
                <span>Go</span>
              </button>
              <button className="flex items-center gap-2 bg-white hover:bg-slate-50 text-slate-600 px-8 py-3 rounded-xl font-bold border border-slate-200 transition-all active:scale-95">
                <X size={18} />
                <span>Reset</span>
              </button>
            </div>
          </div>
        </div>

        {/* Results Section */}
        <div className="bg-white rounded-2xl border border-slate-200 shadow-sm min-h-[400px] flex flex-col">
          <div className="p-6 border-b border-slate-100">
            <h2 className="text-sm font-bold text-slate-800">Results</h2>
            <p className="text-[13px] text-slate-400 mt-0.5">0 records matching your filters</p>
          </div>
          
          <div className="flex-1 flex flex-col items-center justify-center p-20 text-center">
            <div className="w-20 h-20 bg-slate-50 rounded-full flex items-center justify-center mb-6 border border-slate-100">
              <Inbox className="text-slate-300" size={32} strokeWidth={1.5} />
            </div>
            <h3 className="text-lg font-bold text-slate-800">No matching communications</h3>
            <p className="text-slate-400 text-sm mt-2 max-w-sm mx-auto leading-relaxed">
              Try loosening your filters or resetting to start over.
            </p>
          </div>
        </div>

      </div>
    </div>
  );
};

export default SimpleCommunicationTracker;
