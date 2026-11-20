import React, { useState, useRef, useEffect, useCallback } from 'react';
import ReactDOM from 'react-dom';
import CheckIcon from '@mui/icons-material/Check';
import KeyboardArrowDownIcon from '@mui/icons-material/KeyboardArrowDown';
import KeyboardArrowUpIcon from '@mui/icons-material/KeyboardArrowUp';
import SearchIcon from '@mui/icons-material/Search';
import CloseIcon from '@mui/icons-material/Close';

/**
 * SearchableSelect
 * Renders the dropdown via a React Portal so it is never clipped
 * by parent overflow:hidden / overflow:auto containers.
 *
 * Props
 *   options     [{ value, label }]
 *   value       string
 *   onChange    (value) => void
 *   placeholder string
 *   header      string
 *   disabled    bool
 */
export default function SearchableSelect({
  options = [],
  value = '0',
  onChange,
  placeholder = '-Select-',
  header = 'Please select',
  disabled = false,
}) {
  const [open,   setOpen]   = useState(false);
  const [search, setSearch] = useState('');
  const [rect,   setRect]   = useState(null);   // trigger bounding rect

  const triggerRef = useRef(null);
  const searchRef  = useRef(null);

  // Recalculate position whenever open changes
  useEffect(() => {
    if (open) {
      const r = triggerRef.current?.getBoundingClientRect();
      if (r) setRect(r);
      setTimeout(() => searchRef.current?.focus(), 30);
    } else {
      setSearch('');
    }
  }, [open]);

  // Close on outside click
  useEffect(() => {
    if (!open) return;
    const handler = (e) => {
      if (
        triggerRef.current && !triggerRef.current.contains(e.target) &&
        !document.getElementById('ss-portal')?.contains(e.target)
      ) {
        setOpen(false);
      }
    };
    document.addEventListener('mousedown', handler);
    return () => document.removeEventListener('mousedown', handler);
  }, [open]);

  // Close on scroll / resize
  useEffect(() => {
    if (!open) return;
    const close = (e) => {
      // Ignore scroll events from within the dropdown itself
      if (e.target.id === 'ss-portal' || e.target.closest?.('#ss-portal')) {
        return;
      }
      setOpen(false);
    };
    window.addEventListener('scroll', close, true);
    window.addEventListener('resize', close);
    return () => {
      window.removeEventListener('scroll', close, true);
      window.removeEventListener('resize', close);
    };
  }, [open]);

  const selectedLabel = options.find((o) => o.value === value)?.label ?? '';
  const isSelected    = value && value !== '0';

  const filtered = search.trim()
    ? options.filter((o) => o.label.toLowerCase().includes(search.trim().toLowerCase()))
    : options;

  const handleSelect = useCallback((val) => {
    onChange(val);
    setOpen(false);
  }, [onChange]);

  // ── Portal dropdown ──────────────────────────────────────────────────────────
  const ITEM_H = 36;
  const VISIBLE = 12; // Increased from 8
  const totalRows = filtered.length + 1;
  const listH = Math.max(Math.min(totalRows, VISIBLE) * ITEM_H, 5 * ITEM_H);

  const dropdown = open && rect && ReactDOM.createPortal(
    <div
      id="ss-portal"
      style={{
        position: 'fixed',
        top:  rect.bottom + 3,
        left: rect.left,
        width: rect.width,
        minWidth: 220,
        zIndex: 99999,
        background: '#fff',
        border: '1px solid #d0d0d0',
        borderRadius: 8,
        boxShadow: '0 6px 24px rgba(0,0,0,0.15)',
        fontFamily: 'inherit',
      }}
    >
      <style>{`
        #ss-portal ul::-webkit-scrollbar { width: 6px; }
        #ss-portal ul::-webkit-scrollbar-track { background: #f5f5f5; border-radius: 0 0 8px 0; }
        #ss-portal ul::-webkit-scrollbar-thumb { background: #ccc; border-radius: 10px; }
        #ss-portal ul::-webkit-scrollbar-thumb:hover { background: #bbb; }
      `}</style>
      {/* Header */}
      <div style={{
        display: 'flex', alignItems: 'center', justifyContent: 'space-between',
        padding: '9px 14px',
        background: '#f5f5f5',
        borderBottom: '1px solid #e8e8e8',
        borderRadius: '8px 8px 0 0',
      }}>
        <span style={{ fontSize: 13, fontWeight: 700, color: '#333' }}>{header}</span>
        <button onClick={() => setOpen(false)} style={{
          background: 'none', border: 'none', cursor: 'pointer',
          padding: 2, lineHeight: 0, color: '#999',
        }}>
          <CloseIcon style={{ fontSize: 15 }} />
        </button>
      </div>

      {/* Search */}
      <div style={{ padding: '8px 10px', borderBottom: '1px solid #efefef' }}>
        <div style={{
          display: 'flex', alignItems: 'center',
          border: '1.5px solid #1976d2', borderRadius: 6,
          padding: '4px 8px', background: '#fff', gap: 5,
        }}>
          <SearchIcon style={{ fontSize: 15, color: '#aaa', flexShrink: 0 }} />
          <input
            ref={searchRef}
            value={search}
            onChange={(e) => setSearch(e.target.value)}
            placeholder="Search..."
            style={{
              flex: 1, border: 'none', outline: 'none',
              fontSize: 13, color: '#333', background: 'transparent',
              padding: 0, minWidth: 0,
            }}
          />
          {search && (
            <button onClick={() => setSearch('')} style={{
              background: 'none', border: 'none', cursor: 'pointer',
              padding: 0, lineHeight: 0, color: '#aaa',
            }}>
              <CloseIcon style={{ fontSize: 13 }} />
            </button>
          )}
        </div>
      </div>

      {/* List */}
      <ul style={{
        height: listH,
        overflowY: totalRows > VISIBLE ? 'scroll' : 'auto',
        margin: 0, padding: 0, listStyle: 'none',
        scrollbarWidth: 'thin', scrollbarColor: '#ccc #f5f5f5',
      }}>
        {/* -Select- */}
        <Row
          label={placeholder}
          isActive={value === '0'}
          isPlaceholder
          height={ITEM_H}
          onClick={() => handleSelect('0')}
        />

        {filtered.length === 0 && (
          <li style={{
            height: ITEM_H, display: 'flex', alignItems: 'center',
            padding: '0 14px', fontSize: 13, color: '#aaa',
          }}>
            No results found
          </li>
        )}

        {filtered.map((opt) => (
          <Row
            key={opt.value}
            label={opt.label}
            isActive={opt.value === value}
            height={ITEM_H}
            onClick={() => handleSelect(opt.value)}
          />
        ))}
      </ul>
    </div>,
    document.body
  );

  return (
    <>
      {/* Trigger */}
      <div
        ref={triggerRef}
        onClick={() => !disabled && setOpen((o) => !o)}
        style={{
          display: 'flex', alignItems: 'center', justifyContent: 'space-between',
          height: 36, padding: '0 10px',
          border: `1px solid ${open ? '#1976d2' : '#c4c4c4'}`,
          borderRadius: 8,
          background: disabled ? '#f5f5f5' : '#fff',
          cursor: disabled ? 'not-allowed' : 'pointer',
          boxSizing: 'border-box',
          boxShadow: open ? '0 0 0 2px rgba(25,118,210,0.18)' : 'none',
          userSelect: 'none',
          transition: 'border-color .15s, box-shadow .15s',
          fontFamily: 'inherit',
        }}
      >
        <span style={{
          flex: 1, overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap',
          fontSize: 13,
          color: isSelected ? '#1a1a2e' : '#aaa',
          fontWeight: isSelected ? 500 : 400,
          marginRight: 4,
        }}>
          {isSelected ? selectedLabel : placeholder}
        </span>
        {open
          ? <KeyboardArrowUpIcon   style={{ fontSize: 18, color: '#666', flexShrink: 0 }} />
          : <KeyboardArrowDownIcon style={{ fontSize: 18, color: '#666', flexShrink: 0 }} />
        }
      </div>

      {dropdown}
    </>
  );
}

// ── Single row ─────────────────────────────────────────────────────────────────
function Row({ label, isActive, isPlaceholder = false, height, onClick }) {
  const [hovered, setHovered] = useState(false);
  return (
    <li
      onClick={onClick}
      onMouseEnter={() => setHovered(true)}
      onMouseLeave={() => setHovered(false)}
      style={{
        display: 'flex', alignItems: 'center', justifyContent: 'space-between',
        height, padding: '0 14px', fontSize: 13, cursor: 'pointer',
        background: isActive ? '#e8f0fe' : hovered ? '#f5f5f5' : '#fff',
        color: isActive ? '#1976d2' : isPlaceholder ? '#aaa' : '#222',
        fontWeight: isActive ? 600 : 400,
        fontStyle: isPlaceholder ? 'italic' : 'normal',
        borderBottom: '1px solid #f5f5f5',
        boxSizing: 'border-box',
        transition: 'background .08s',
      }}
    >
      <span style={{
        overflow: 'hidden', textOverflow: 'ellipsis',
        whiteSpace: 'nowrap', flex: 1,
      }}>
        {label}
      </span>
      {isActive && (
        <CheckIcon style={{ fontSize: 15, color: '#1976d2', flexShrink: 0, marginLeft: 6 }} />
      )}
    </li>
  );
}
