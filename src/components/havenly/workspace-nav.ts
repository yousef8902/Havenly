import { Activity, CalendarCheck, Home, LayoutDashboard, PlusCircle, Users } from "lucide-react";
import type { NavItem } from "./shells";

export const hostNav: NavItem[] = [
  { to: "/host", label: "Dashboard", Icon: LayoutDashboard, exact: true },
  { to: "/host/bookings", label: "Booking requests", Icon: CalendarCheck },
  { to: "/host/properties/new", label: "Add property", Icon: PlusCircle },
];

export const adminNav: NavItem[] = [
  { to: "/admin", label: "Overview", Icon: Home, exact: true },
  { to: "/admin/activity", label: "Activity log", Icon: Activity },
];

export const adminIcons = { Users };
