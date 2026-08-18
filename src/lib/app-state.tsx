import { createContext, useCallback, useContext, useMemo, useState, type ReactNode } from "react";
import { bookings as seedBookings, type Booking, type BookingStatus } from "./data";

type AppState = {
  favorites: string[];
  toggleFavorite: (id: string) => void;
  isFavorite: (id: string) => boolean;
  bookings: Booking[];
  addBooking: (b: Booking) => void;
  setBookingStatus: (id: string, status: BookingStatus) => void;
};

const Ctx = createContext<AppState | null>(null);

export function AppStateProvider({ children }: { children: ReactNode }) {
  const [favorites, setFavorites] = useState<string[]>(["casa-fiora", "pine-hollow"]);
  const [list, setList] = useState<Booking[]>(seedBookings);

  const toggleFavorite = useCallback((id: string) => {
    setFavorites((f) => (f.includes(id) ? f.filter((x) => x !== id) : [...f, id]));
  }, []);

  const value = useMemo<AppState>(
    () => ({
      favorites,
      toggleFavorite,
      isFavorite: (id: string) => favorites.includes(id),
      bookings: list,
      addBooking: (b) => setList((prev) => [b, ...prev]),
      setBookingStatus: (id, status) =>
        setList((prev) => prev.map((b) => (b.id === id ? { ...b, status } : b))),
    }),
    [favorites, toggleFavorite, list],
  );

  return <Ctx.Provider value={value}>{children}</Ctx.Provider>;
}

export function useAppState() {
  const ctx = useContext(Ctx);
  if (!ctx) throw new Error("useAppState must be used inside AppStateProvider");
  return ctx;
}