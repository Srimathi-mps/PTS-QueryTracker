import React from 'react';
import { 
  Settings, 
  Grid, 
  Layers, 
  FileJson, 
  ShieldCheck,
  ChevronRight,
  Plus
} from 'lucide-react';

const Configuration: React.FC = () => {
  const configModules = [
    { title: 'Problem Types', description: 'Configure high-level query classifications and codes.', icon: Grid, count: 12, color: 'bg-blue-600' },
    { title: 'Problem Categories', description: 'Define specific sub-categories for precise tracking.', icon: Layers, count: 48, color: 'bg-indigo-600' },
    { title: 'System Roles', description: 'Manage security levels and user permission mappings.', icon: ShieldCheck, count: 4, color: 'bg-emerald-600' },
    { title: 'File System', description: 'Global configuration for naming and storage paths.', icon: FileJson, count: 1, color: 'bg-amber-600' },
  ];

  return (
    <div className="animate-in fade-in duration-700">
      <div className="mb-10">
        <h1 className="text-3xl font-black text-slate-900 tracking-tight">System Configuration</h1>
        <p className="text-slate-500 font-medium">Administrator dashboard for global system parameters.</p>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 gap-8">
        {configModules.map((module, i) => (
          <div key={i} className="bg-white border border-slate-200 p-10 rounded-[2.5rem] hover:border-blue-500 hover:shadow-2xl hover:shadow-blue-500/10 transition-all group cursor-pointer shadow-sm">
            <div className="flex items-start justify-between mb-8">
              <div className={`p-5 ${module.color} rounded-2xl shadow-lg shadow-slate-200 transition-transform group-hover:scale-110`}>
                <module.icon className="text-white" size={32} />
              </div>
              <button className="p-3 bg-slate-50 border border-slate-200 rounded-xl text-slate-400 hover:text-slate-900 transition-colors">
                <Plus size={24} />
              </button>
            </div>
            <h3 className="text-2xl font-black text-slate-900 mb-3">{module.title}</h3>
            <p className="text-slate-500 text-sm leading-relaxed mb-10 font-medium">{module.description}</p>
            <div className="flex items-center justify-between pt-8 border-t border-slate-100">
              <span className="text-[10px] font-black text-slate-400 uppercase tracking-widest">{module.count} Active Entries</span>
              <div className="flex items-center gap-2 text-blue-600 font-black text-sm">
                <span>Open Module</span>
                <ChevronRight size={18} />
              </div>
            </div>
          </div>
        ))}
      </div>
    </div>
  );
};

export default Configuration;
