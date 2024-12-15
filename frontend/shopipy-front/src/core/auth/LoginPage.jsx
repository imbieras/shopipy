import { useState } from "react"
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";

export default function LoginPage({onLogin}) {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("")
  const [isLoading, setIsLoading] = useState(false)

  const handleSubmit = async (e) => {
    e.preventDefault()
    setIsLoading(true)

    try {
      await onLogin(username, password)
      // Reset form on success if needed
      setUsername("");
      setPassword("");
    } catch (error) {
      console.error("Login error:", error)
    } finally {
      setIsLoading(false)
    }
  }

  return (
    <div className="w-full max-w-md space-y-6 p-6 rounded-lg border bg-card">
      <div className="space-y-2 text-center">
        <h2 className="text-2xl font-bold">Welcome back</h2>
        <p className="text-sm text-muted-foreground">
          Enter your credentials to access your account
        </p>
      </div>
      <form onSubmit={handleSubmit} className="space-y-4">
        <div className="space-y-2">
          <Input
            type="username"
            placeholder="Username"
            value={username}
            onChange={(e) => setUsername(e.target.value)}
            required
            className="w-full"
          />
        </div>
        <div className="space-y-2">
          <Input
            type="password"
            placeholder="Password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            required
            className="w-full"
          />
        </div>
        <Button
          type="submit"
          className="w-full"
          disabled={isLoading}
        >
          {isLoading ? "Signing in..." : "Sign in"}
        </Button>
      </form>
    </div>
  )
}