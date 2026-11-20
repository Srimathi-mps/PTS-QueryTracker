import React, { useState, useEffect } from 'react';
import { 
  Search, 
  FileText, 
  Barcode, 
  ChevronDown, 
  RefreshCw,
  Plus,
  Info,
  Trash2,
  Edit3,
  Paperclip,
  Send,
  MessageSquare,
  X,
  ClipboardList,
  AlertCircle,
  Loader2,
  Filter
} from 'lucide-react';
import { ProblemService } from '../services/api';

const CommunicationTracker: React.FC = () => {
  const [barcode, setBarcode] = useState('');
  const [showNewQuery, setShowNewQuery] = useState(false);
  const [problems, setProblems] = useState<any[]>([]);
  const [loading, setLoading] = useState(true);
  const [customers, setCustomers] = useState<any[]>([]);
  const [journals, setJournals] = useState<any[]>([]);
  const [articles, setArticles] = useState<any[]>([]);
  const [departments, setDepartments] = useState<any[]>([]);

  // Filter state
  const [filters, setFilters] = useState({
    customer: 0,
    journal: 0,
    article: '',
    isbn: '',
    stage: '',
    department: ''
  });

  // New Query state
  const [newQuery, setNewQuery] = useState({
    departmentId: 0,
    problemTypeId: 0,
    categoryId: 0,
    remarks: '',
    proceed: 'YES'
  });
  
  useEffect(() => {
    fetchInitialData();
  }, []);

  const fetchInitialData = async () => {
    try {
      setLoading(true);
      const [customersData, departmentsData] = await Promise.all([
        ProblemService.getCustomers(),
        ProblemService.getDepartments()
      ]);
      setCustomers(customersData);
      setDepartments(departmentsData);
      
      // Fetch initial history
      const historyData = await ProblemService.getProblemHistory(0, 0, '');
      setProblems(historyData);
    } catch (error) {
      console.error('Failed to fetch data:', error);
    } finally {
      setLoading(false);
    }
  };

  // Cascade: Customer -> Journals
  useEffect(() => {
    if (filters.customer > 0) {
      const fetchJournals = async () => {
        try {
          const data = await ProblemService.getJournals(filters.customer);
          setJournals(data);
          setFilters(prev => ({ ...prev, journal: 0, article: '' })); // Reset downstream
          setArticles([]);
        } catch (error) {
          console.error('Failed to fetch journals:', error);
        }
      };
      fetchJournals();
    } else {
      setJournals([]);
      setArticles([]);
    }
  }, [filters.customer]);

  // Cascade: Journal -> Articles
  useEffect(() => {
    if (filters.customer > 0 && filters.journal > 0) {
      const fetchArticles = async () => {
        try {
          const data = await ProblemService.getArticles(filters.customer, filters.journal);
          setArticles(data);
          setFilters(prev => ({ ...prev, article: '' })); // Reset downstream
        } catch (error) {
          console.error('Failed to fetch articles:', error);
        }
      };
      fetchArticles();
    } else {
      setArticles([]);
    }
  }, [filters.customer, filters.journal]);

  const handleSearch = async () => {
    try {
      setLoading(true);
      const historyData = await ProblemService.getProblemHistory(
        filters.customer,
        filters.journal,
        filters.article
      );
      setProblems(historyData);
    } catch (error) {
      console.error('Search failed:', error);
    } finally {
      setLoading(false);
    }
  };

  const handleSendQuery = async () => {
    if (!filters.customer || !filters.journal || !filters.article) {
      alert('Please select Customer, Project, and Job first.');
      return;
    }

    try {
      setLoading(true);
      await ProblemService.raiseProblem({
        customerCode: filters.customer,
        journalCode: filters.journal,
        article: filters.article,
        remarks: newQuery.remarks,
        userName: 'MAC014393'
      });
      setShowNewQuery(false);
      setNewQuery({ departmentId: 0, problemTypeId: 0, categoryId: 0, remarks: '', proceed: 'YES' });
      handleSearch(); // Refresh list
    } catch (error) {
      console.error('Failed to send query:', error);
      alert('Failed to send query. Please try again.');
    } finally {
      setLoading(false);
    }
  };

  const handleReset = () => {
    setBarcode('');
    setFilters({
      customer: 0,
      journal: 0,
      article: '',
      isbn: '',
      stage: '',
      department: ''
    });
    fetchInitialData();
  };

  const activeFilterCount = Object.values(filters).filter(val => val !== 0 && val !== '').length + (barcode ? 1 : 0);

  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-50 to-blue-50/50 p-6 space-y-6 font-sans">
      
      {/* HEADER SECTION */}
      <div className="flex flex-col md:flex-row md:items-center justify-between p-5 bg-white/70 backdrop-blur-md rounded-2xl shadow-lg border border-white/50 transition-all duration-300 hover:shadow-xl">
        <div className="flex items-start gap-4">
          <div className="p-3 bg-gradient-to-br from-blue-500 to-purple-600 rounded-xl shadow-md text-white">
            <MessageSquare size={24} />
          </div>
          <div>
            <h1 className="text-2xl font-bold text-slate-900 tracking-tight">Communication Tracker</h1>
            <p className="text-sm text-slate-500 mt-1">Filter customer communications across projects, jobs and production stages</p>
          </div>
        </div>
        <div className="mt-4 md:mt-0 flex items-center gap-3">
          <div className="flex items-center gap-2 px-4 py-2 bg-blue-50 rounded-full border border-blue-100 shadow-sm">
            <span className="text-sm font-medium text-slate-600">Total Records:</span>
            <span className="text-sm font-bold text-blue-700 bg-white px-2 py-0.5 rounded-full shadow-sm">{problems.length}</span>
          </div>
        </div>
      </div>

      {/* FILTERS SECTION */}
      <div className="bg-white/70 backdrop-blur-md rounded-2xl shadow-lg border border-white/50 p-6 transition-all duration-300 hover:shadow-xl">
        <div className="flex items-center justify-between mb-6 border-b border-slate-200/60 pb-4">
          <div className="flex items-center gap-2">
            <Filter size={20} className="text-blue-600" />
            <h2 className="text-lg font-bold text-slate-800">Filters</h2>
            {activeFilterCount > 0 && (
              <span className="ml-2 px-2.5 py-0.5 bg-blue-100 text-blue-700 text-xs font-bold rounded-full">
                {activeFilterCount} active
              </span>
            )}
          </div>
        </div>

        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          {/* Barcode */}
          <div className="lg:col-span-3 mb-2">
            <label className="block text-xs font-bold text-slate-500 mb-2 uppercase tracking-wider">Barcode Search</label>
            <div className="relative">
              <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                <Barcode size={18} className="text-slate-400" />
              </div>
              <input
                type="text"
                value={barcode}
                onChange={(e) => setBarcode(e.target.value)}
                placeholder="Customer | Project | Job"
                className="w-full pl-10 pr-4 py-3 bg-white/50 border border-slate-200 rounded-xl text-sm text-slate-900 focus:outline-none focus:ring-2 focus:ring-blue-500/50 focus:border-blue-500 transition-all shadow-sm"
              />
            </div>
          </div>

          <div>
            <label className="block text-xs font-bold text-slate-500 mb-2 uppercase tracking-wider">Customer</label>
            <select
              value={filters.customer}
              onChange={(e) => setFilters({ ...filters, customer: Number(e.target.value) })}
              className="w-full px-4 py-3 bg-white/50 border border-slate-200 rounded-xl text-sm text-slate-900 focus:outline-none focus:ring-2 focus:ring-blue-500/50 focus:border-blue-500 transition-all shadow-sm appearance-none"
            >
              <option value={0}>- Select Customer -</option>
              {customers.map((c: any) => (
                <option key={c.id || c.code} value={c.id || c.code}>{c.name || c.code}</option>
              ))}
            </select>
          </div>

          <div>
            <label className="block text-xs font-bold text-slate-500 mb-2 uppercase tracking-wider">Project</label>
            <select
              value={filters.journal}
              onChange={(e) => setFilters({ ...filters, journal: Number(e.target.value) })}
              disabled={!filters.customer}
              className="w-full px-4 py-3 bg-white/50 border border-slate-200 rounded-xl text-sm text-slate-900 focus:outline-none focus:ring-2 focus:ring-blue-500/50 focus:border-blue-500 transition-all shadow-sm disabled:opacity-50 appearance-none"
            >
              <option value={0}>- Select Project -</option>
              {journals.map((j: any) => (
                <option key={j.id || j.code} value={j.id || j.code}>{j.name || j.code}</option>
              ))}
            </select>
          </div>

          <div>
            <label className="block text-xs font-bold text-slate-500 mb-2 uppercase tracking-wider">Job</label>
            <select
              value={filters.article}
              onChange={(e) => setFilters({ ...filters, article: e.target.value })}
              disabled={!filters.journal}
              className="w-full px-4 py-3 bg-white/50 border border-slate-200 rounded-xl text-sm text-slate-900 focus:outline-none focus:ring-2 focus:ring-blue-500/50 focus:border-blue-500 transition-all shadow-sm disabled:opacity-50 appearance-none"
            >
              <option value="">- Select Job -</option>
              {articles.map((a: any) => (
                <option key={a.id || a.article} value={a.article}>{a.article}</option>
              ))}
            </select>
          </div>

          <div>
            <label className="block text-xs font-bold text-slate-500 mb-2 uppercase tracking-wider">ISBN</label>
            <input
              type="text"
              value={filters.isbn}
              onChange={(e) => setFilters({ ...filters, isbn: e.target.value })}
              placeholder="Enter ISBN"
              className="w-full px-4 py-3 bg-white/50 border border-slate-200 rounded-xl text-sm text-slate-900 focus:outline-none focus:ring-2 focus:ring-blue-500/50 focus:border-blue-500 transition-all shadow-sm"
            />
          </div>

          <div>
            <label className="block text-xs font-bold text-slate-500 mb-2 uppercase tracking-wider">Stage</label>
            <select
              value={filters.stage}
              onChange={(e) => setFilters({ ...filters, stage: e.target.value })}
              className="w-full px-4 py-3 bg-white/50 border border-slate-200 rounded-xl text-sm text-slate-900 focus:outline-none focus:ring-2 focus:ring-blue-500/50 focus:border-blue-500 transition-all shadow-sm appearance-none"
            >
              <option value="">- Select Stage -</option>
              <option value="S100">S100</option>
              <option value="S200">S200</option>
              <option value="S300">S300</option>
            </select>
          </div>

          <div>
            <label className="block text-xs font-bold text-slate-500 mb-2 uppercase tracking-wider">Department</label>
            <select
              value={filters.department}
              onChange={(e) => setFilters({ ...filters, department: e.target.value })}
              className="w-full px-4 py-3 bg-white/50 border border-slate-200 rounded-xl text-sm text-slate-900 focus:outline-none focus:ring-2 focus:ring-blue-500/50 focus:border-blue-500 transition-all shadow-sm appearance-none"
            >
              <option value="">- Select Department -</option>
              {departments.map((d: any) => (
                <option key={d.id} value={d.id}>{d.name}</option>
              ))}
            </select>
          </div>
        </div>

        <div className="mt-8 flex items-center justify-end gap-4">
          <button
            onClick={handleReset}
            className="px-6 py-2.5 rounded-full border border-slate-200 text-slate-600 font-semibold text-sm hover:bg-slate-50 transition-all duration-200 hover:shadow-sm"
          >
            Reset
          </button>
          <button
            onClick={handleSearch}
            className="flex items-center gap-2 px-8 py-2.5 bg-gradient-to-r from-blue-600 to-purple-600 text-white rounded-full font-semibold text-sm hover:shadow-lg hover:shadow-blue-500/30 hover:-translate-y-0.5 transition-all duration-200"
          >
            <Search size={18} />
            Go
          </button>
        </div>
      </div>

      {/* RESULTS SECTION */}
      <div className="bg-white/80 backdrop-blur-md rounded-2xl shadow-lg border border-white/50 p-6 transition-all duration-300 hover:shadow-xl mt-6">
        <div className="flex items-center justify-between mb-6">
          <h2 className="text-xl font-bold text-slate-800">Results</h2>
          {problems.length > 0 && (
            <span className="text-sm font-medium text-slate-500">Showing {problems.length} records</span>
          )}
        </div>

        {loading ? (
          <div className="py-20 flex flex-col items-center justify-center animate-pulse">
            <Loader2 className="w-10 h-10 text-blue-500 animate-spin mb-4" />
            <div className="text-slate-500 font-medium">Loading communications...</div>
          </div>
        ) : problems.length > 0 ? (
          <div className="overflow-x-auto rounded-xl border border-slate-200/60 shadow-sm">
            <table className="min-w-full text-sm text-left whitespace-nowrap">
              <thead className="bg-slate-50/80 backdrop-blur-sm border-b border-slate-200/60 sticky top-0 z-10">
                <tr>
                  {['Code', 'Job', 'Remarks', 'Status', 'Type', 'Actions', 'Date'].map((header) => (
                    <th key={header} className="px-6 py-4 font-bold text-slate-600 uppercase tracking-wider text-xs">
                      {header}
                    </th>
                  ))}
                </tr>
              </thead>
              <tbody className="divide-y divide-slate-100">
                {problems.map((row, i) => (
                  <tr key={i} className="bg-white hover:bg-blue-50/50 transition-colors group">
                    <td className="px-6 py-4 font-medium text-slate-800">{row.code || 'N/A'}</td>
                    <td className="px-6 py-4 text-slate-600">{row.job || row.article || 'N/A'}</td>
                    <td className="px-6 py-4 text-slate-600 max-w-xs truncate" title={row.remarks || 'No remarks'}>
                      {row.remarks || <span className="text-slate-400 italic">No remarks</span>}
                    </td>
                    <td className="px-6 py-4">
                      <span className={`inline-flex items-center px-2.5 py-1 text-xs font-bold rounded-full border ${
                        row.status === 'Raised' 
                          ? 'bg-orange-50 text-orange-600 border-orange-200' 
                          : row.status === 'Closed' || row.status === 'Completed'
                          ? 'bg-green-50 text-green-600 border-green-200'
                          : row.status === 'Pending'
                          ? 'bg-yellow-50 text-yellow-600 border-yellow-200'
                          : row.status === 'Overdue'
                          ? 'bg-red-50 text-red-600 border-red-200'
                          : 'bg-blue-50 text-blue-600 border-blue-200'
                      }`}>
                        {row.status || 'Unknown'}
                      </span>
                    </td>
                    <td className="px-6 py-4 text-slate-600">{row.type || 'General'}</td>
                    <td className="px-6 py-4">
                      <div className="flex items-center gap-2 opacity-70 group-hover:opacity-100 transition-opacity">
                        <button className="p-1.5 rounded-lg bg-white border border-slate-200 text-slate-500 hover:text-blue-600 hover:border-blue-300 hover:bg-blue-50 transition-all shadow-sm">
                          <Edit3 size={16} />
                        </button>
                        <button className="p-1.5 rounded-lg bg-white border border-slate-200 text-slate-500 hover:text-red-600 hover:border-red-300 hover:bg-red-50 transition-all shadow-sm">
                          <Trash2 size={16} />
                        </button>
                      </div>
                    </td>
                    <td className="px-6 py-4 text-slate-500 text-xs font-medium">{row.date || 'Today'}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        ) : (
          <div className="py-20 flex flex-col items-center justify-center text-center animate-in fade-in duration-500">
            <div className="w-20 h-20 bg-blue-50 rounded-full flex items-center justify-center mb-4 shadow-inner">
              <ClipboardList className="w-10 h-10 text-blue-300" />
            </div>
            <h3 className="text-xl font-bold text-slate-800 mb-2">No matching communications</h3>
            <p className="text-slate-500 max-w-sm">
              We couldn't find any records matching your current filters. Try adjusting your search criteria or resetting the filters.
            </p>
            <button 
              onClick={handleReset}
              className="mt-6 px-6 py-2 bg-white border border-slate-200 text-slate-600 rounded-full font-semibold text-sm hover:bg-slate-50 hover:text-blue-600 transition-colors shadow-sm"
            >
              Clear Filters
            </button>
          </div>
        )}
      </div>

    </div>
  );
};

export default CommunicationTracker;
