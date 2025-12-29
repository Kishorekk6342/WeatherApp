// ✅ Wait for Supabase to load and configure with session persistence
(function () {
    function initSupabase() {
        // Check if Supabase is loaded
        if (!window.supabase) {
            console.log('Waiting for Supabase to load...');
            setTimeout(initSupabase, 100);
            return;
        }

        // Create Supabase client ONLY ONCE with session persistence
        if (!window.supabaseClient) {
            window.supabaseClient = window.supabase.createClient(
                "https://ukibpplazxsuqhildshh.supabase.co",
                "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InVraWJwcGxhenhzdXFoaWxkc2hoIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NjQ3MjA2NTEsImV4cCI6MjA4MDI5NjY1MX0.MjZ7OAt7BgYNzUosxi4cAMdMPRt6liAGPgMNV3U_1p0",
                {
                    auth: {
                        // ✅ Store session in localStorage (persists across refresh)
                        storage: window.localStorage,
                        storageKey: 'supabase.auth.token',
                        autoRefreshToken: true,
                        persistSession: true,
                        detectSessionInUrl: true
                    }
                }
            );
            console.log('✅ Supabase client initialized with session persistence');
        }

        const supabase = window.supabaseClient;

        // 🔐 LOGIN
        window.sbLogin = async (email, password) => {
            try {
                const { data, error } = await supabase.auth.signInWithPassword({
                    email,
                    password
                });
                if (error) {
                    console.error('❌ Login error:', error);
                    throw error;
                }
                console.log('✅ Login successful:', data.user.email);
                return {
                    id: data.user.id,
                    email: data.user.email,
                    phone: data.user.phone,
                    createdAt: data.user.created_at,
                    lastSignInAt: data.user.last_sign_in_at
                };
            } catch (error) {
                console.error('❌ Login exception:', error);
                throw error;
            }
        };

        // 📝 REGISTER
        window.sbRegister = async (email, password, username) => {
            const { data, error } = await supabase.auth.signUp({
                email,
                password,
                options: {
                    data: {
                        username: username   // ✅ THIS IS CRITICAL
                    }
                }
            });

            if (error) throw error;
            return data;
        };




        // 🚪 LOGOUT
        window.sbLogout = async () => {
            try {
                const { error } = await supabase.auth.signOut();
                if (error) {
                    console.error('❌ Logout error:', error);
                    throw error;
                }
                console.log('✅ Logout successful');
                return true;
            } catch (error) {
                console.error('❌ Logout exception:', error);
                throw error;
            }
        };

        // 👤 GET CURRENT USER (from persisted session)
        window.sbGetUser = async function () {
            try {
                const { data: { user }, error } = await supabase.auth.getUser();

                if (error) {
                    console.error('❌ Get user error:', error);
                    return null;
                }

                if (!user) {
                    console.log('⚠️ No user found');
                    return null;
                }

                console.log('✅ User retrieved:', user.email);

                return {
                    id: user.id,
                    email: user.email,
                    phone: user.phone,
                    createdAt: user.created_at,
                    lastSignInAt: user.last_sign_in_at
                };
            } catch (ex) {
                console.error('❌ Exception in sbGetUser:', ex);
                return null;
            }
        };

        // 🔄 GET SESSION (useful for debugging)
        window.sbGetSession = async function () {
            try {
                const { data: { session }, error } = await supabase.auth.getSession();

                if (error) {
                    console.error('❌ Get session error:', error);
                    return null;
                }

                if (!session) {
                    console.log('⚠️ No active session');
                    return null;
                }

                console.log('✅ Session retrieved:', {
                    hasAccessToken: !!session.access_token,
                    hasRefreshToken: !!session.refresh_token,
                    userId: session.user?.id
                });

                return {
                    access_token: session.access_token,
                    refresh_token: session.refresh_token,
                    expires_in: session.expires_in,
                    token_type: session.token_type,
                    user: {
                        id: session.user?.id,
                        email: session.user?.email
                    }
                };
            } catch (ex) {
                console.error('❌ Exception in sbGetSession:', ex);
                return null;
            }
        };

        // 🔑 GET ACCESS TOKEN (for C# Supabase client)
        window.sbGetAccessToken = async function () {
            try {
                const { data: { session }, error } = await supabase.auth.getSession();

                if (error || !session) {
                    console.error('❌ No session for access token');
                    return null;
                }

                return session.access_token;
            } catch (ex) {
                console.error('❌ Exception in sbGetAccessToken:', ex);
                return null;
            }
        };

        // 🎧 AUTH STATE CHANGE LISTENER (optional - for real-time updates)
        window.sbOnAuthStateChange = (callback) => {
            return supabase.auth.onAuthStateChange((event, session) => {
                console.log('Auth state changed:', event, session?.user?.email);
                if (callback && typeof callback === 'function') {
                    callback(event, session);
                }
            });
        };

        console.log('✅ Supabase auth functions ready');
    }

    // Start initialization
    initSupabase();
})();