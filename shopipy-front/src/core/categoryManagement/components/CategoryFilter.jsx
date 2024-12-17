import {
    Select,
    SelectContent,
    SelectItem,
    SelectTrigger,
    SelectValue,
} from "@/components/ui/select";

export function CategoryFilter({ categories, selectedCategory, onSelectCategory }) {
    return (
      <Select
        value={selectedCategory?.toString() || "all"}
        onValueChange={(value) => onSelectCategory(value === "all" ? null : parseInt(value))}
      >
        <SelectTrigger className="w-[180px]">
          <SelectValue placeholder="Select category" />
        </SelectTrigger>
        <SelectContent>
          <SelectItem value="all">All categories</SelectItem>
          {categories.map((category) => (
            <SelectItem
              key={category.categoryId}
              value={category.categoryId.toString()}
            >
              {category.name}
            </SelectItem>
          ))}
        </SelectContent>
      </Select>
    );
}