// SPDX-FileCopyrightText: 2021 Ayoub Kaanich <kayoub5@live.com>
//
// SPDX-License-Identifier: MIT

namespace SharpPcap.Tunneling.Unix
{
    enum SocketIoctl
    {
        SIOCGIFNAME = 0x8910, /* get iface name		*/
        SIOCSIFLINK = 0x8911, /* set iface channel		*/
        SIOCGIFCONF = 0x8912, /* get iface list		*/
        SIOCGIFFLAGS = 0x8913, /* get flags			*/
        SIOCSIFFLAGS = 0x8914, /* set flags			*/
        SIOCGIFADDR = 0x8915, /* get PA address		*/
        SIOCSIFADDR = 0x8916, /* set PA address		*/
        SIOCGIFDSTADDR = 0x8917, /* get remote PA address	*/
        SIOCSIFDSTADDR = 0x8918, /* set remote PA address	*/
        SIOCGIFBRDADDR = 0x8919, /* get broadcast PA address	*/
        SIOCSIFBRDADDR = 0x891a, /* set broadcast PA address	*/
        SIOCGIFNETMASK = 0x891b, /* get network PA mask		*/
        SIOCSIFNETMASK = 0x891c, /* set network PA mask		*/
        SIOCGIFMETRIC = 0x891d, /* get metric			*/
        SIOCSIFMETRIC = 0x891e, /* set metric			*/
        SIOCGIFMEM = 0x891f, /* get memory address (BSD)	*/
        SIOCSIFMEM = 0x8920, /* set memory address (BSD)	*/
        SIOCGIFMTU = 0x8921, /* get MTU size			*/
        SIOCSIFMTU = 0x8922, /* set MTU size			*/
        SIOCSIFNAME = 0x8923, /* set interface name */
        SIOCSIFHWADDR = 0x8924, /* set hardware address 	*/
        SIOCGIFENCAP = 0x8925, /* get/set encapsulations       */
        SIOCSIFENCAP = 0x8926,
        SIOCGIFHWADDR = 0x8927, /* Get hardware address		*/
        SIOCGIFSLAVE = 0x8929, /* Driver slaving support	*/
        SIOCSIFSLAVE = 0x8930,
        SIOCADDMULTI = 0x8931, /* Multicast address lists	*/
        SIOCDELMULTI = 0x8932,
        SIOCGIFINDEX = 0x8933, /* name -> if_index mapping	*/
        SIOCSIFPFLAGS = 0x8934, /* set/get extended flags set	*/
        SIOCGIFPFLAGS = 0x8935,
        SIOCDIFADDR = 0x8936, /* delete PA address		*/
        SIOCSIFHWBROADCAST = 0x8937, /* set hardware broadcast addr	*/
        SIOCGIFCOUNT = 0x8938, /* get number of devices */
        SIOCGIFBR = 0x8940, /* Bridging support		*/
        SIOCSIFBR = 0x8941, /* Set bridging options 	*/
        SIOCGIFTXQLEN = 0x8942, /* Get the tx queue length	*/
        SIOCSIFTXQLEN = 0x8943, /* Set the tx queue length 	*/

        SIOCETHTOOL = 0x8946, /* Ethtool interface		*/
        SIOCGMIIPHY = 0x8947, /* Get address of MII PHY in use. */
        SIOCGMIIREG = 0x8948, /* Read MII PHY register.	*/
        SIOCSMIIREG = 0x8949, /* Write MII PHY register.	*/
        SIOCWANDEV = 0x894A, /* get/set netdev parameters	*/
        /* ARP cache control calls. */

        SIOCDARP = 0x8953, /* delete ARP table entry	*/
        SIOCGARP = 0x8954, /* get ARP table entry		*/
        SIOCSARP = 0x8955, /* set ARP table entry		*/
        /* RARP cache control calls. */
        SIOCDRARP = 0x8960, /* delete RARP table entry	*/
        SIOCGRARP = 0x8961, /* get RARP table entry		*/
        SIOCSRARP = 0x8962, /* set RARP table entry		*/
        /* Driver configuration calls */
        SIOCGIFMAP = 0x8970, /* Get device parameters	*/
        SIOCSIFMAP = 0x8971, /* Set device parameters	*/
        /* DLCI configuration calls */
        SIOCADDDLCI = 0x8980, /* Create new DLCI device	*/
        SIOCDELDLCI = 0x8981, /* Delete DLCI device		*/
        SIOCGIFVLAN = 0x8982, /* 802.1Q VLAN support		*/
        SIOCSIFVLAN = 0x8983, /* Set 802.1Q VLAN options 	*/
        /* bonding calls */
        SIOCBONDENSLAVE = 0x8990, /* enslave a device to the bond */
        SIOCBONDRELEASE = 0x8991, /* release a slave from the bond*/
        SIOCBONDSETHWADDR = 0x8992, /* set the hw addr of the bond  */
        SIOCBONDSLAVEINFOQUERY = 0x8993, /* rtn info about slave state   */
        SIOCBONDINFOQUERY = 0x8994, /* rtn info about bond state    */
        SIOCBONDCHANGEACTIVE = 0x8995, /* update to a new active slave */

        /* bridge calls */
        SIOCBRADDBR = 0x89a0, /* create new bridge device     */
        SIOCBRDELBR = 0x89a1, /* remove bridge device         */
        SIOCBRADDIF = 0x89a2, /* add interface to bridge      */
        SIOCBRDELIF = 0x89a3, /* remove interface from bridge */
    }
}
