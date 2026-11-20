import React from 'react';
import { 
  ClipboardList, 
  Filter, 
  Download, 
  Search,
  Calendar,
  CheckCircle2,
  Clock,
  AlertCircle
} from 'lucide-react';

const DueForClosing: React.FC = () => {
  return (
    <div className="animate-in fade-in duration-700">
      <div className="flex justify-between items-center mb-8">
        <div>
          <h1 className="text-3xl font-black text-slate-900 tracking-tight">Due For Closing</h1>
          <p className="text-slate-500 font-medium">Review pending queries and track resolution timelines.</p>
        </div>
        <button className="flex items-center gap-3 bg-emerald-600 hover:bg-emerald-700 text-white px-8 py-3 rounded-2xl font-black shadow-lg shadow-emerald-600/20 transition-all">
          <Download size={20} />
          <span>Export Excel</span>
        </button>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-4 gap-8 mb-10">
        {[
          { label: 'Total Pending', value: '42', icon: Clock, color: 'text-amber-500', bg: 'bg-amber-50' },
          { label: 'Overdue', value: '12', icon: AlertCircle, color: 'text-rose-500', bg: 'bg-rose-50' },
          { label: 'Resolved Today', value: '15', icon: CheckCircle2, color: 'text-emerald-500', bg: 'bg-emerald-50' },
          { label: 'Avg Close Time', value: '2.4 Days', icon: Calendar, color: 'text-blue-500', bg: 'bg-blue-50' },
        ].map((stat, i) => (
          <div key={i} className="bg-white border border-slate-200 p-8 rounded-[2rem] shadow-sm hover:shadow-md transition-shadow">
            <div className="flex items-center gap-4 mb-5">
              <div className={`p-3 ${stat.bg} rounded-xl ${stat.color}`}>
                <stat.icon size={22} />
              </div>
              <span className="text-[10px] font-black text-slate-400 uppercase tracking-[0.2em]">{stat.label}</span>
            </div>
            <p className="text-3xl font-black text-slate-900">{stat.value}</p>
          </div>
        ))}
      </div>

      <div className="bg-white border border-slate-200 rounded-[2.5rem] overflow-hidden shadow-xl shadow-slate-200/50">
        <div className="p-8 border-b border-slate-100 flex justify-between items-center bg-slate-50/50">
          <div className="flex items-center gap-4 bg-white border border-slate-200 px-6 py-3 rounded-2xl w-[400px] shadow-sm">
            <Search className="text-slate-400" size={20} />
            <input type="text" placeholder="Filter report data..." className="bg-transparent border-none text-sm text-slate-900 focus:outline-none w-full font-semibold" />
          </div>
          <button className="flex items-center gap-2 text-slate-500 hover:text-slate-900 font-bold text-sm transition-colors">
            <Filter size={18} />
            <span>Show Filters</span>
          </button>
        </div>
        <div className="p-24 flex flex-col items-center justify-center text-slate-400">
          <div className="w-24 h-24 bg-slate-50 rounded-full flex items-center justify-center mb-6 border-2 border-dashed border-slate-200">
            <ClipboardList size={40} className="text-slate-200" />
          </div>
          <p className="text-xl font-black text-slate-900">All queries resolved!</p>
          <p className="text-sm font-medium mt-2">There are currently no queries due for closing.</p>
        </div>
      </div>
    </div>
  );
};

export default DueForClosing;
