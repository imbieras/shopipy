//this shit temporary we using something else i forgor what
import { Input } from "@/components/ui/input"

export function SearchBar({ onSearch }) {
  return (
    <Input
      type="text"
      placeholder="Search products..."
      onChange={(e) => onSearch(e.target.value)}
      className="w-full"
    />
  )
}

