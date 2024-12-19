// src/components/ui/textarea.jsx
import React from 'react';

export function Textarea(props) {
  return (
    <textarea
      className="border border-gray-300 rounded-md p-2 w-full"
      {...props}
    />
  );
}
